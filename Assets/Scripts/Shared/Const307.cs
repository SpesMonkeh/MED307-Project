// 17/09/2023

using UnityEngine;

namespace P307.Shared
{
	public static class Const307
	{
		public const int
			ZERO					= 0,
			ONE 					= 1,
			TWO 					= 2,
			THREE					= 3,
			FOUR					= 4,
			FIVE					= 5,
			SIX						= 6,
			SEVEN					= 7,
			EIGHT					= 8,
			NINE					= 9,
			TEN						= 10,
			THIRTEEN				= 13,
			SEVENTEEN				= 17,
			HAND_LANDMARK_INDICES	= 20,
			HAND_LANDMARK_COUNT		= 21,
			ONE_HUNDRED				= 100,
			ONE_THOUSAND			= 1000;
		
		public const float
			DEG2_RAD				= .017453292f,
			RAD_2_DEG				= 57.29578f,
			
			NEGATIVE_INFINITY		= -.1f / .0f,
			PI						= 3.1415927f,
			TAU						= 6.2831854f,
			HALF					= .5f,
			INFINITY				= .1f / .0f;

		public const double
			ONE_MILLIONTH			= 1e-6;
		
		public const string
			P307_TAG				= "P307",
			NULL_STRING				= "Null";

#region MEDIAPIPE-RELATED VALUES

		public const string
			MEDIAPIPE_SOLUTION		= "MEDIAPIPE - Solution";

#endregion
		
#region HAND LANDMARK TAGS

		public const string // HAND REGIONS
			WRIST			= "WRIST",
			THUMB			= "THUMB",
			INDEX_FINGER	= "INDEX_FINGER",
			MIDDLE_FINGER	= "MIDDLE_FINGER",
			RING_FINGER		= "RING_FINGER",
			PINKY			= "PINKY";
		
		public const string
			CMC				= "_CMC",
			MCP				= "_MCP",
			IP				= "_IP",
			PIP				= "_PIP",
			DIP				= "_DIP",
			TIP				= "_TIP";

		public const string
			THUMB_CMC			= THUMB + CMC,
			THUMB_MCP			= THUMB + MCP,
			THUMB_IP			= THUMB + IP,
			THUMB_TIP			= THUMB + TIP,
			INDEX_FINGER_MCP	= INDEX_FINGER + MCP,
			INDEX_FINGER_PIP	= INDEX_FINGER + PIP,
			INDEX_FINGER_DIP	= INDEX_FINGER + DIP,
			INDEX_FINGER_TIP	= INDEX_FINGER + TIP,
			MIDDLE_FINGER_MCP	= MIDDLE_FINGER + MCP,
			MIDDLE_FINGER_PIP	= MIDDLE_FINGER + PIP,
			MIDDLE_FINGER_DIP	= MIDDLE_FINGER + DIP,
			MIDDLE_FINGER_TIP	= MIDDLE_FINGER + TIP,
			RING_FINGER_MCP		= RING_FINGER + MCP,
			RING_FINGER_PIP		= RING_FINGER + PIP,
			RING_FINGER_DIP		= RING_FINGER + DIP,
			RING_FINGER_TIP		= RING_FINGER + TIP,
			PINKY_MCP			= PINKY + MCP,
			PINKY_PIP			= PINKY + PIP,
			PINKY_DIP			= PINKY + DIP,
			PINKY_TIP			= PINKY + TIP;

#endregion
	}
}