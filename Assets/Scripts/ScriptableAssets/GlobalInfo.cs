using UnityEngine;
public static class GlobalInfo
{
	private static GlobalInfoData globalInfoData;

	public static GlobalInfoData GlobalInfoData
	{
		get
		{
			if (globalInfoData == null)
				globalInfoData = Resources.Load<GlobalInfoData>("GlobalInfoData");

			return globalInfoData;
		}
	}

	public static KinematicCharacterController.Examples.ExampleCharacterController SelectedCharacter
	{
		get; set;
	}
}
