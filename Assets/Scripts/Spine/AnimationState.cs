using System;
using System.Collections.Generic;
using System.Text;

namespace Spine
{
	public class AnimationState
	{
		public delegate void TrackEntryDelegate(TrackEntry trackEntry);

		public delegate void TrackEntryEventDelegate(TrackEntry trackEntry, Event e);

		private static readonly Animation EmptyAnimation = new Animation("<empty>", new ExposedList<Timeline>(), 0f);

		private AnimationStateData data;

		private readonly ExposedList<TrackEntry> tracks = new ExposedList<TrackEntry>();

		private readonly HashSet<int> propertyIDs = new HashSet<int>();

		private readonly ExposedList<Event> events = new ExposedList<Event>();

		private readonly EventQueue queue;

		private bool animationsChanged;

		private float timeScale = 1f;

		private Pool<TrackEntry> trackEntryPool = new Pool<TrackEntry>();

		public AnimationStateData Data => data;

		public ExposedList<TrackEntry> Tracks => tracks;

		public float TimeScale
		{
			get
			{
				return timeScale;
			}
			set
			{
				timeScale = value;
			}
		}

		public event TrackEntryDelegate Start;

		public event TrackEntryDelegate Interrupt;

		public event TrackEntryDelegate End;

		public event TrackEntryDelegate Dispose;

		public event TrackEntryDelegate Complete;

		public event TrackEntryEventDelegate Event;

		public AnimationState(AnimationStateData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "data cannot be null.");
			}
			this.data = data;
			queue = new EventQueue(this, HandleAnimationsChanged, trackEntryPool);
		}

		private void HandleAnimationsChanged()
		{
			animationsChanged = true;
		}

		public void Update(float delta)
		{
			delta *= timeScale;
			TrackEntry[] items = tracks.Items;
			int i = 0;
			for (int count = tracks.Count; i < count; i++)
			{
				TrackEntry trackEntry = items[i];
				if (trackEntry == null)
				{
					continue;
				}
				trackEntry.animationLast = trackEntry.nextAnimationLast;
				trackEntry.trackLast = trackEntry.nextTrackLast;
				float num = delta * trackEntry.timeScale;
				if (trackEntry.delay > 0f)
				{
					trackEntry.delay -= num;
					if (trackEntry.delay > 0f)
					{
						continue;
					}
					num = 0f - trackEntry.delay;
					trackEntry.delay = 0f;
				}
				TrackEntry trackEntry2 = trackEntry.next;
				if (trackEntry2 != null)
				{
					float num2 = trackEntry.trackLast - trackEntry2.delay;
					if (num2 >= 0f)
					{
						trackEntry2.delay = 0f;
						trackEntry2.trackTime = num2 + delta * trackEntry2.timeScale;
						trackEntry.trackTime += num;
						SetCurrent(i, trackEntry2, interrupt: true);
						while (trackEntry2.mixingFrom != null)
						{
							trackEntry2.mixTime += num;
							trackEntry2 = trackEntry2.mixingFrom;
						}
						continue;
					}
				}
				else if (trackEntry.trackLast >= trackEntry.trackEnd && trackEntry.mixingFrom == null)
				{
					items[i] = null;
					queue.End(trackEntry);
					DisposeNext(trackEntry);
					continue;
				}
				UpdateMixingFrom(trackEntry, delta);
				trackEntry.trackTime += num;
			}
			queue.Drain();
		}

		private void UpdateMixingFrom(TrackEntry entry, float delta)
		{
			TrackEntry mixingFrom = entry.mixingFrom;
			if (mixingFrom != null)
			{
				UpdateMixingFrom(mixingFrom, delta);
				if (entry.mixTime >= entry.mixDuration && mixingFrom.mixingFrom == null && entry.mixTime > 0f)
				{
					entry.mixingFrom = null;
					queue.End(mixingFrom);
					return;
				}
				mixingFrom.animationLast = mixingFrom.nextAnimationLast;
				mixingFrom.trackLast = mixingFrom.nextTrackLast;
				mixingFrom.trackTime += delta * mixingFrom.timeScale;
				entry.mixTime += delta * entry.timeScale;
			}
		}

		public void Apply(Skeleton skeleton)
		{
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}
			if (animationsChanged)
			{
				AnimationsChanged();
			}
			ExposedList<Event> exposedList = events;
			TrackEntry[] items = tracks.Items;
			int i = 0;
			for (int count = tracks.Count; i < count; i++)
			{
				TrackEntry trackEntry = items[i];
				if (trackEntry == null || trackEntry.delay > 0f)
				{
					continue;
				}
				float num = trackEntry.alpha;
				if (trackEntry.mixingFrom != null)
				{
					num *= ApplyMixingFrom(trackEntry, skeleton);
				}
				else if (trackEntry.trackTime >= trackEntry.trackEnd)
				{
					num = 0f;
				}
				float animationLast = trackEntry.animationLast;
				float animationTime = trackEntry.AnimationTime;
				int count2 = trackEntry.animation.timelines.Count;
				ExposedList<Timeline> timelines = trackEntry.animation.timelines;
				Timeline[] items2 = timelines.Items;
				if (num == 1f)
				{
					for (int j = 0; j < count2; j++)
					{
						items2[j].Apply(skeleton, animationLast, animationTime, exposedList, 1f, setupPose: true, mixingOut: false);
					}
				}
				else
				{
					bool flag = trackEntry.timelinesRotation.Count == 0;
					if (flag)
					{
						trackEntry.timelinesRotation.EnsureCapacity(timelines.Count << 1);
					}
					float[] items3 = trackEntry.timelinesRotation.Items;
					bool[] items4 = trackEntry.timelinesFirst.Items;
					for (int k = 0; k < count2; k++)
					{
						Timeline timeline = items2[k];
						RotateTimeline rotateTimeline = timeline as RotateTimeline;
						if (rotateTimeline != null)
						{
							ApplyRotateTimeline(rotateTimeline, skeleton, animationTime, num, items4[k], items3, k << 1, flag);
						}
						else
						{
							timeline.Apply(skeleton, animationLast, animationTime, exposedList, num, items4[k], mixingOut: false);
						}
					}
				}
				QueueEvents(trackEntry, animationTime);
				exposedList.Clear(clearArray: false);
				trackEntry.nextAnimationLast = animationTime;
				trackEntry.nextTrackLast = trackEntry.trackTime;
			}
			queue.Drain();
		}

		private float ApplyMixingFrom(TrackEntry entry, Skeleton skeleton)
		{
			TrackEntry mixingFrom = entry.mixingFrom;
			if (mixingFrom.mixingFrom != null)
			{
				ApplyMixingFrom(mixingFrom, skeleton);
			}
			float num;
			if (entry.mixDuration == 0f)
			{
				num = 1f;
			}
			else
			{
				num = entry.mixTime / entry.mixDuration;
				if (num > 1f)
				{
					num = 1f;
				}
			}
			ExposedList<Event> exposedList = (!(num < mixingFrom.eventThreshold)) ? null : events;
			bool flag = num < mixingFrom.attachmentThreshold;
			bool flag2 = num < mixingFrom.drawOrderThreshold;
			float animationLast = mixingFrom.animationLast;
			float animationTime = mixingFrom.AnimationTime;
			ExposedList<Timeline> timelines = mixingFrom.animation.timelines;
			Timeline[] items = timelines.Items;
			int count = timelines.Count;
			ExposedList<bool> timelinesFirst = mixingFrom.timelinesFirst;
			bool[] items2 = timelinesFirst.Items;
			float alpha = mixingFrom.alpha * entry.mixAlpha * (1f - num);
			bool flag3 = entry.timelinesRotation.Count == 0;
			if (flag3)
			{
				entry.timelinesRotation.EnsureCapacity(timelines.Count << 1);
			}
			float[] items3 = entry.timelinesRotation.Items;
			for (int i = 0; i < count; i++)
			{
				Timeline timeline = items[i];
				bool flag4 = items2[i];
				RotateTimeline rotateTimeline = timeline as RotateTimeline;
				if (rotateTimeline != null)
				{
					ApplyRotateTimeline(rotateTimeline, skeleton, animationTime, alpha, flag4, items3, i << 1, flag3);
				}
				else if (flag4 || ((flag || !(timeline is AttachmentTimeline)) && (flag2 || !(timeline is DrawOrderTimeline))))
				{
					timeline.Apply(skeleton, animationLast, animationTime, exposedList, alpha, flag4, mixingOut: true);
				}
			}
			if (entry.mixDuration > 0f)
			{
				QueueEvents(mixingFrom, animationTime);
			}
			events.Clear(clearArray: false);
			mixingFrom.nextAnimationLast = animationTime;
			mixingFrom.nextTrackLast = mixingFrom.trackTime;
			return num;
		}

		private static void ApplyRotateTimeline(RotateTimeline rotateTimeline, Skeleton skeleton, float time, float alpha, bool setupPose, float[] timelinesRotation, int i, bool firstFrame)
		{
			if (firstFrame)
			{
				timelinesRotation[i] = 0f;
			}
			if (alpha == 1f)
			{
				rotateTimeline.Apply(skeleton, 0f, time, null, 1f, setupPose, mixingOut: false);
				return;
			}
			Bone bone = skeleton.bones.Items[rotateTimeline.boneIndex];
			float[] frames = rotateTimeline.frames;
			if (time < frames[0])
			{
				if (setupPose)
				{
					bone.rotation = bone.data.rotation;
				}
				return;
			}
			float num;
			if (time >= frames[frames.Length - 2])
			{
				num = bone.data.rotation + frames[frames.Length + -1];
			}
			else
			{
				int num2 = Animation.BinarySearch(frames, time, 2);
				float num3 = frames[num2 + -1];
				float num4 = frames[num2];
				float curvePercent = rotateTimeline.GetCurvePercent((num2 >> 1) - 1, 1f - (time - num4) / (frames[num2 + -2] - num4));
				num = frames[num2 + 1] - num3;
				num -= (float)((16384 - (int)(16384.499999999996 - (double)(num / 360f))) * 360);
				num = num3 + num * curvePercent + bone.data.rotation;
				num -= (float)((16384 - (int)(16384.499999999996 - (double)(num / 360f))) * 360);
			}
			float num5 = (!setupPose) ? bone.rotation : bone.data.rotation;
			float num6 = num - num5;
			float num7;
			if (num6 == 0f)
			{
				num7 = timelinesRotation[i];
			}
			else
			{
				num6 -= (float)((16384 - (int)(16384.499999999996 - (double)(num6 / 360f))) * 360);
				float num8;
				float value;
				if (firstFrame)
				{
					num8 = 0f;
					value = num6;
				}
				else
				{
					num8 = timelinesRotation[i];
					value = timelinesRotation[i + 1];
				}
				bool flag = num6 > 0f;
				bool flag2 = num8 >= 0f;
				if (Math.Sign(value) != Math.Sign(num6) && Math.Abs(value) <= 90f)
				{
					if (Math.Abs(num8) > 180f)
					{
						num8 += (float)(360 * Math.Sign(num8));
					}
					flag2 = flag;
				}
				num7 = num6 + num8 - num8 % 360f;
				if (flag2 != flag)
				{
					num7 += (float)(360 * Math.Sign(num8));
				}
				timelinesRotation[i] = num7;
			}
			timelinesRotation[i + 1] = num6;
			num5 += num7 * alpha;
			bone.rotation = num5 - (float)((16384 - (int)(16384.499999999996 - (double)(num5 / 360f))) * 360);
		}

		private void QueueEvents(TrackEntry entry, float animationTime)
		{
			float animationStart = entry.animationStart;
			float animationEnd = entry.animationEnd;
			float num = animationEnd - animationStart;
			float num2 = entry.trackLast % num;
			ExposedList<Event> exposedList = events;
			Event[] items = exposedList.Items;
			int i = 0;
			int count;
			for (count = exposedList.Count; i < count; i++)
			{
				Event @event = items[i];
				if (@event.time < num2)
				{
					break;
				}
				if (!(@event.time > animationEnd))
				{
					queue.Event(entry, @event);
				}
			}
			if (entry.loop ? (num2 > entry.trackTime % num) : (animationTime >= animationEnd && entry.animationLast < animationEnd))
			{
				queue.Complete(entry);
			}
			for (; i < count; i++)
			{
				Event event2 = items[i];
				if (!(event2.time < animationStart))
				{
					queue.Event(entry, items[i]);
				}
			}
		}

		public void ClearTracks()
		{
			bool drainDisabled = queue.drainDisabled;
			queue.drainDisabled = true;
			int i = 0;
			for (int count = tracks.Count; i < count; i++)
			{
				ClearTrack(i);
			}
			tracks.Clear();
			queue.drainDisabled = drainDisabled;
			queue.Drain();
		}

		public void ClearTrack(int trackIndex)
		{
			if (trackIndex >= tracks.Count)
			{
				return;
			}
			TrackEntry trackEntry = tracks.Items[trackIndex];
			if (trackEntry == null)
			{
				return;
			}
			queue.End(trackEntry);
			DisposeNext(trackEntry);
			TrackEntry trackEntry2 = trackEntry;
			while (true)
			{
				TrackEntry mixingFrom = trackEntry2.mixingFrom;
				if (mixingFrom == null)
				{
					break;
				}
				queue.End(mixingFrom);
				trackEntry2.mixingFrom = null;
				trackEntry2 = mixingFrom;
			}
			tracks.Items[trackEntry.trackIndex] = null;
			queue.Drain();
		}

		private void SetCurrent(int index, TrackEntry current, bool interrupt)
		{
			TrackEntry trackEntry = ExpandToIndex(index);
			tracks.Items[index] = current;
			if (trackEntry != null)
			{
				if (interrupt)
				{
					queue.Interrupt(trackEntry);
				}
				current.mixingFrom = trackEntry;
				current.mixTime = 0f;
				trackEntry.timelinesRotation.Clear();
				if (trackEntry.mixingFrom != null && trackEntry.mixDuration > 0f)
				{
					current.mixAlpha *= Math.Min(trackEntry.mixTime / trackEntry.mixDuration, 1f);
				}
			}
			queue.Start(current);
		}

		public TrackEntry SetAnimation(int trackIndex, string animationName, bool loop)
		{
			Animation animation = data.skeletonData.FindAnimation(animationName);
			if (animation == null)
			{
				throw new ArgumentException("Animation not found: " + animationName, "animationName");
			}
			return SetAnimation(trackIndex, animation, loop);
		}

		public TrackEntry SetAnimation(int trackIndex, Animation animation, bool loop)
		{
			if (animation == null)
			{
				throw new ArgumentNullException("animation", "animation cannot be null.");
			}
			bool interrupt = true;
			TrackEntry trackEntry = ExpandToIndex(trackIndex);
			if (trackEntry != null)
			{
				if (trackEntry.nextTrackLast == -1f)
				{
					tracks.Items[trackIndex] = trackEntry.mixingFrom;
					queue.Interrupt(trackEntry);
					queue.End(trackEntry);
					DisposeNext(trackEntry);
					trackEntry = trackEntry.mixingFrom;
					interrupt = false;
				}
				else
				{
					DisposeNext(trackEntry);
				}
			}
			TrackEntry trackEntry2 = NewTrackEntry(trackIndex, animation, loop, trackEntry);
			SetCurrent(trackIndex, trackEntry2, interrupt);
			queue.Drain();
			return trackEntry2;
		}

		public TrackEntry AddAnimation(int trackIndex, string animationName, bool loop, float delay)
		{
			Animation animation = data.skeletonData.FindAnimation(animationName);
			if (animation == null)
			{
				throw new ArgumentException("Animation not found: " + animationName, "animationName");
			}
			return AddAnimation(trackIndex, animation, loop, delay);
		}

		public TrackEntry AddAnimation(int trackIndex, Animation animation, bool loop, float delay)
		{
			if (animation == null)
			{
				throw new ArgumentNullException("animation", "animation cannot be null.");
			}
			TrackEntry trackEntry = ExpandToIndex(trackIndex);
			if (trackEntry != null)
			{
				while (trackEntry.next != null)
				{
					trackEntry = trackEntry.next;
				}
			}
			TrackEntry trackEntry2 = NewTrackEntry(trackIndex, animation, loop, trackEntry);
			if (trackEntry == null)
			{
				SetCurrent(trackIndex, trackEntry2, interrupt: true);
				queue.Drain();
			}
			else
			{
				trackEntry.next = trackEntry2;
				if (delay <= 0f)
				{
					float num = trackEntry.animationEnd - trackEntry.animationStart;
					delay = ((num == 0f) ? 0f : (delay + (num * (float)(1 + (int)(trackEntry.trackTime / num)) - data.GetMix(trackEntry.animation, animation))));
				}
			}
			trackEntry2.delay = delay;
			return trackEntry2;
		}

		public TrackEntry SetEmptyAnimation(int trackIndex, float mixDuration)
		{
			TrackEntry trackEntry = SetAnimation(trackIndex, EmptyAnimation, loop: false);
			trackEntry.mixDuration = mixDuration;
			trackEntry.trackEnd = mixDuration;
			return trackEntry;
		}

		public TrackEntry AddEmptyAnimation(int trackIndex, float mixDuration, float delay)
		{
			if (delay <= 0f)
			{
				delay -= mixDuration;
			}
			TrackEntry trackEntry = AddAnimation(trackIndex, EmptyAnimation, loop: false, delay);
			trackEntry.mixDuration = mixDuration;
			trackEntry.trackEnd = mixDuration;
			return trackEntry;
		}

		public void SetEmptyAnimations(float mixDuration)
		{
			bool drainDisabled = queue.drainDisabled;
			queue.drainDisabled = true;
			int i = 0;
			for (int count = tracks.Count; i < count; i++)
			{
				TrackEntry trackEntry = tracks.Items[i];
				if (trackEntry != null)
				{
					SetEmptyAnimation(i, mixDuration);
				}
			}
			queue.drainDisabled = drainDisabled;
			queue.Drain();
		}

		private TrackEntry ExpandToIndex(int index)
		{
			if (index < tracks.Count)
			{
				return tracks.Items[index];
			}
			while (index >= tracks.Count)
			{
				tracks.Add(null);
			}
			return null;
		}

		private TrackEntry NewTrackEntry(int trackIndex, Animation animation, bool loop, TrackEntry last)
		{
			TrackEntry trackEntry = trackEntryPool.Obtain();
			trackEntry.trackIndex = trackIndex;
			trackEntry.animation = animation;
			trackEntry.loop = loop;
			trackEntry.eventThreshold = 0f;
			trackEntry.attachmentThreshold = 0f;
			trackEntry.drawOrderThreshold = 0f;
			trackEntry.animationStart = 0f;
			trackEntry.animationEnd = animation.Duration;
			trackEntry.animationLast = -1f;
			trackEntry.nextAnimationLast = -1f;
			trackEntry.delay = 0f;
			trackEntry.trackTime = 0f;
			trackEntry.trackLast = -1f;
			trackEntry.nextTrackLast = -1f;
			trackEntry.trackEnd = float.MaxValue;
			trackEntry.timeScale = 1f;
			trackEntry.alpha = 1f;
			trackEntry.mixAlpha = 1f;
			trackEntry.mixTime = 0f;
			trackEntry.mixDuration = ((last != null) ? data.GetMix(last.animation, animation) : 0f);
			return trackEntry;
		}

		private void DisposeNext(TrackEntry entry)
		{
			for (TrackEntry next = entry.next; next != null; next = next.next)
			{
				queue.Dispose(next);
			}
			entry.next = null;
		}

		private void AnimationsChanged()
		{
			animationsChanged = false;
			HashSet<int> hashSet = propertyIDs;
			int i = 0;
			int count = tracks.Count;
			hashSet.Clear();
			for (; i < count; i++)
			{
				TrackEntry trackEntry = tracks.Items[i];
				if (trackEntry != null)
				{
					SetTimelinesFirst(trackEntry);
					i++;
					break;
				}
			}
			for (; i < count; i++)
			{
				TrackEntry trackEntry2 = tracks.Items[i];
				if (trackEntry2 != null)
				{
					CheckTimelinesFirst(trackEntry2);
				}
			}
		}

		private void SetTimelinesFirst(TrackEntry entry)
		{
			if (entry.mixingFrom != null)
			{
				SetTimelinesFirst(entry.mixingFrom);
				CheckTimelinesUsage(entry);
				return;
			}
			HashSet<int> hashSet = propertyIDs;
			ExposedList<Timeline> timelines = entry.animation.timelines;
			int count = timelines.Count;
			entry.timelinesFirst.EnsureCapacity(count);
			bool[] items = entry.timelinesFirst.Items;
			Timeline[] items2 = timelines.Items;
			for (int i = 0; i < count; i++)
			{
				hashSet.Add(items2[i].PropertyId);
				items[i] = true;
			}
		}

		private void CheckTimelinesFirst(TrackEntry entry)
		{
			if (entry.mixingFrom != null)
			{
				CheckTimelinesFirst(entry.mixingFrom);
			}
			CheckTimelinesUsage(entry);
		}

		private void CheckTimelinesUsage(TrackEntry entry)
		{
			HashSet<int> hashSet = propertyIDs;
			ExposedList<Timeline> timelines = entry.animation.timelines;
			int count = timelines.Count;
			ExposedList<bool> timelinesFirst = entry.timelinesFirst;
			timelinesFirst.EnsureCapacity(count);
			bool[] items = timelinesFirst.Items;
			Timeline[] items2 = timelines.Items;
			for (int i = 0; i < count; i++)
			{
				items[i] = hashSet.Add(items2[i].PropertyId);
			}
		}

		public TrackEntry GetCurrent(int trackIndex)
		{
			return (trackIndex < tracks.Count) ? tracks.Items[trackIndex] : null;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			for (int count = tracks.Count; i < count; i++)
			{
				TrackEntry trackEntry = tracks.Items[i];
				if (trackEntry != null)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(trackEntry.ToString());
				}
			}
			return (stringBuilder.Length != 0) ? stringBuilder.ToString() : "<none>";
		}

		internal void OnStart(TrackEntry entry)
		{
			if (this.Start != null)
			{
				this.Start(entry);
			}
		}

		internal void OnInterrupt(TrackEntry entry)
		{
			if (this.Interrupt != null)
			{
				this.Interrupt(entry);
			}
		}

		internal void OnEnd(TrackEntry entry)
		{
			if (this.End != null)
			{
				this.End(entry);
			}
		}

		internal void OnDispose(TrackEntry entry)
		{
			if (this.Dispose != null)
			{
				this.Dispose(entry);
			}
		}

		internal void OnComplete(TrackEntry entry)
		{
			if (this.Complete != null)
			{
				this.Complete(entry);
			}
		}

		internal void OnEvent(TrackEntry entry, Event e)
		{
			if (this.Event != null)
			{
				this.Event(entry, e);
			}
		}
	}
}
