namespace Spine
{
	public interface Timeline
	{
		int PropertyId
		{
			get;
		}

		void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> events, float alpha, bool setupPose, bool mixingOut);
	}
}
