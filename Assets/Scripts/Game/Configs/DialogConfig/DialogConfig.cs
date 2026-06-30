using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/DialogConfig", fileName = "DialogConfig")]
public class DialogConfig : ScriptableObject
{
    [Serializable]
    public struct DialogInLevel
    {
        public DialogData[] DialogData;
        public RuntimeAnimatorController AnimatorController;
        public int Location;
        public int Level;
    }
    public DialogInLevel[] dialogInLevels;
    public float Interval;
    public float AutoSkipDelay;
    public bool Cheak(int location, int level)
    {
        for (int i = 0; i < dialogInLevels.Length; i++)
        {
            if(dialogInLevels[i].Location == location && dialogInLevels[i].Level == level) 
                return true;
        }
        return false;
    }
    public DialogData[] GetDialogData(int location, int level)
    {
        DialogData[] dialogDatas = new DialogData[0];
        for (int i = 0; i < dialogInLevels.Length; i++)
        {
            if (dialogInLevels[i].Location == location && dialogInLevels[i].Level == level)
            {
                dialogDatas = dialogInLevels[i].DialogData;
            }  
        }
        return dialogDatas;
    }
    public RuntimeAnimatorController GetAnimatorController(int location, int level)
    {
        for (int i = 0; i < dialogInLevels.Length; i++)
        {
            if (dialogInLevels[i].Location == location && dialogInLevels[i].Level == level)
            {
                return dialogInLevels[i].AnimatorController;
            }
        }
        return null;
    }
}