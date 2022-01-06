using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;

namespace BlockKitMod
{
    static class Globals
    {
        public static bool Initialized = false;
    }


    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    class ModInit : MySessionComponentBase
    {
        private IMyHudNotification _Notify = null;
        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {
            base.Init(sessionComponent);
            MyAPIGateway.Session.SessionSettings.InventorySizeMultiplier = 40;
        }

        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();
            Globals.Initialized = true;
        }

        private void ShowOnHud(string text, int displayms = 10000)
        {
            if (_Notify == null)
            {
                _Notify = MyAPIGateway.Utilities.CreateNotification(text, displayms, MyFontEnum.Red);
            }
            else
            {
                _Notify.Text = text;
                _Notify.ResetAliveTime();
            }

            _Notify.Show();
        }
    }
}
