using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace WindowsFocusViewer
{
    public static class ParagraphExt
    {
        private static readonly HashSet<Run> _WaitHash=new HashSet<Run>();

        public static async void WaitStart(this Paragraph paragraph, Run run)
        {
            if (!_WaitHash.Contains(run))
            {
                paragraph.Inlines.Add(run);
                _WaitHash.Add(run);
                while (_WaitHash.Contains(run))
                {
                    switch (run.Text.Length)
                    {
                        case 0:
                            run.Text = ".";
                            break;
                        case 1:
                            run.Text = "..";
                            break;
                        case 2:
                            run.Text = "...";
                            break;
                        default:
                            run.Text = ".";
                            break;
                    }

                    await Task.Delay(200);
                }
            }
        }

        public static void WaitStop(this Paragraph paragraph, Run run)
        {
            paragraph.Inlines.Remove(run);
            _WaitHash.Remove(run);
        }

        public static void WriteLine(this Paragraph paragraph, string text, Brush foreground = null)
        {
            var enter = paragraph.Inlines.Count == 0 ? "" : "\r\n";
            var run = new Run
            {
                Text = enter+text,
                Foreground = foreground??Brushes.White
            };
            paragraph.Inlines.Add(run);
        }
    }
}