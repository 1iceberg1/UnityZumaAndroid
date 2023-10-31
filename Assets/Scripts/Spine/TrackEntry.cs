using System;

namespace Spine
{
	public class TrackEntry : Pool<TrackEntry>.IPoolable
	{
		internal Animation animation;

		internal TrackEntry next;

		internal TrackEntry mixingFrom;

		internal int trackIndex;

		internal bool loop;

		internal float eventThreshold;

		internal float attachmentThreshold;

		internal float drawOrderThreshold;

		internal float animationStart;

		internal float animationEnd;

		internal float animationLast;

		internal float nextAnimationLast;

		internal float delay;

		internal float trackTime;

		internal float trackLast;

		internal float nextTrackLast;

		internal float trackEnd;

		internal float timeScale = 1f;

		internal float alpha;

		internal float mixTime;

		internal float mixDuration;

		internal float mixAlpha;

		internal readonly ExposedList<bool> timelinesFirst = new ExposedList<bool>();

		internal readonly ExposedList<float> timelinesRotation = new ExposedList<float>();

		public int TrackIndex => trackIndex;

		public Animation Animation => animation;

		public bool Loop
		{
			get
			{
				return loop;
			}
			set
			{
				loop = value;
			}
		}

		public float Delay
		{
			get
			{
				return delay;
			}
			set
			{
				delay = value;
			}
		}

		public float TrackTime
		{
			get
			{
				return trackTime;
			}
			set
			{
				trackTime = value;
			}
		}

		public float TrackEnd
		{
			get
			{
				return trackEnd;
			}
			set
			{
				trackEnd = value;
			}
		}

		public float AnimationStart
		{
			get
			{
				return animationStart;
			}
			set
			{
				animationStart = value;
			}
		}

		public float AnimationEnd => animationEnd;

		public float AnimationLast
		{
			get
			{
				return animationLast;
			}
			set
			{
				animationLast = value;
				nextAnimationLast = value;
			}
		}

		public float AnimationTime
		{
			get
			{
				if (loop)
				{
					float num = animationEnd - animationStart;
					if (num == 0f)
					{
						return animationStart;
					}
					return trackTime % num + animationStart;
				}
				return Math.Min(trackTime + animationStart, animationEnd);
			}
		}

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

		public float Alpha
		{
			get
			{
				return alpha;
			}
			set
			{
				alpha = value;
			}
		}

		public float EventThreshold
		{
			get
			{
				return eventThreshold;
			}
			set
			{
				eventThreshold = value;
			}
		}

		public float AttachmentThreshold
		{
			get
			{
				return attachmentThreshold;
			}
			set
			{
				attachmentThreshold = value;
			}
		}

		public float DrawOrderThreshold
		{
			get
			{
				return drawOrderThreshold;
			}
			set
			{
				drawOrderThreshold = value;
			}
		}

		public TrackEntry Next => next;

		public bool IsComplete => trackTime >= animationEnd - animationStart;

		public float MixTime
		{
			get
			{
				return mixTime;
			}
			set
			{
				mixTime = value;
			}
		}

		public float MixDuration
		{
			get
			{
				return mixDuration;
			}
			set
			{
				mixDuration = value;
			}
		}

		public TrackEntry MixingFrom => mixingFrom;

		public event AnimationState.TrackEntryDelegate Start;

		public event AnimationState.TrackEntryDelegate Interrupt;

		public event AnimationState.TrackEntryDelegate End;

		public event AnimationState.TrackEntryDelegate Dispose;

		public event AnimationState.TrackEntryDelegate Complete;

		public event AnimationState.TrackEntryEventDelegate Event;

		public void Reset()
		{
			next = null;
			mixingFrom = null;
			animation = null;
			timelinesFirst.Clear();
			timelinesRotation.Clear();
			this.Start = null;
			this.Interrupt = null;
			this.End = null;
			this.Dispose = null;
			this.Complete = null;
			this.Event = null;
		}

		internal void OnStart()
		{
			if (this.Start != null)
			{
				this.Start(this);
			}
		}

		internal void OnInterrupt()
		{
			if (this.Interrupt != null)
			{
				this.Interrupt(this);
			}
		}

		internal void OnEnd()
		{
			if (this.End != null)
			{
				this.End(this);
			}
		}

		internal void OnDispose()
		{
			if (this.Dispose != null)
			{
				this.Dispose(this);
			}
		}

		internal void OnComplete()
		{
			if (this.Complete != null)
			{
				this.Complete(this);
			}
		}

		internal void OnEvent(Event e)
		{
			if (this.Event != null)
			{
				this.Event(this, e);
			}
		}

		public void ResetRotationDirections()
		{
			timelinesRotation.Clear();
		}

		public override string ToString()
		{
			return (animation != null) ? animation.name : "<none>";
		}
	}
}
