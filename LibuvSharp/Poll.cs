using System;
using System.Runtime.InteropServices;

namespace LibuvSharp
{
	public enum PollEvent : int
	{
		Read = 1,
		Write = 2,
	}

	public class Poll : Handle
	{
		delegate void poll_callback(IntPtr handle, int status, int events);

		[DllImport("uv", CallingConvention = CallingConvention.Cdecl)]
		static extern int uv_poll_init(IntPtr loop, IntPtr handle, int fd);

		[DllImport("uv", CallingConvention = CallingConvention.Cdecl)]
		static extern int uv_poll_start(IntPtr handle, int events, poll_callback callback);

		[DllImport("uv", CallingConvention = CallingConvention.Cdecl)]
		static extern int uv_poll_stop(IntPtr handle);

		public Poll(int fd)
			: this(Loop.Constructor, fd)
		{
		}

		public Poll(Loop loop, int fd)
			: base(loop, HandleType.UV_POLL)
		{
			int r = uv_poll_init(loop.NativeHandle, NativeHandle, fd);
			Ensure.Success(r);

			poll_cb += pollcallback;
		}

		public void Start(PollEvent events)
		{
			int r = uv_poll_start(NativeHandle, (int)events, poll_cb);
			Ensure.Success(r);
		}

		public void Stop()
		{
			int r = uv_poll_stop(NativeHandle);
			Ensure.Success(r);

			Event = null;
		}

		event poll_callback poll_cb;

		void pollcallback(IntPtr handle, int status, int events)
		{
			OnEvent((PollEvent)events);
		}

		public event Action<PollEvent> Event;

		void OnEvent(PollEvent events)
		{
			if (Event != null) {
				Event(events);
			}
		}
	}
}

