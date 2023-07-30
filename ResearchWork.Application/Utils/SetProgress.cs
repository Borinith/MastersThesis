// Ignore Spelling: Utils

using Microsoft.WindowsAPICodePack.Taskbar;

namespace ResearchWork.Application.Utils
{
    public static class SetProgress
    {
        public static void SetProgressValue(int currentValue, int maximumValue)
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
            TaskbarManager.Instance.SetProgressValue(currentValue, maximumValue);
        }
    }
}