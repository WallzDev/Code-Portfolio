using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

[Serializable]
[Title("Get Playtime from PlaytimeManager")]
[Description("Gets the playtime from the PlaytimeManager and stores it in a variable")]

[Category("Save System/Get Playtime from PlaytimeManager")]

[Keywords("Save", "Time", "Playtime", "Remember", "Get", "float")]

[Image(typeof(IconNumber), ColorTheme.Type.Black)]
public class GetPlaytimeFromManager : Instruction
{
    public PropertySetNumber target = new PropertySetNumber();
    protected override Task Run(Args args)
    {
        target.Set(PlaytimeManager.Instance.TotalPlayTime, args);
        return DefaultResult;
    }
}
