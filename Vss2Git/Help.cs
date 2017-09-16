using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NDesk.Options;
using System.IO;

namespace Hpdi.Vss2Git
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }
        public void Display(StringBuilder sb)
        {
            this.helpText.Text = sb.ToString();
            this.helpText.Select(0, 0);
            this.ShowDialog();
        }

        public void ShowHelp(OptionSet arguments)
        {
            StringBuilder sb = new StringBuilder(2048);
            var stringBuilder = new StringBuilder();
            using (TextWriter writer = new StringWriter(stringBuilder))
            {
                arguments.WriteOptionDescriptions(writer);
            }

            string fullString = stringBuilder.ToString();
            sb.AppendLine(string.Format("USAGE: {0} [OPTIONS]+ message", AppDomain.CurrentDomain.FriendlyName));
            sb.AppendLine("Vss2Git is a Windows GUI application that exports all or parts of an existing Microsoft Visual SourceSafe 6.0(VSS)(Wikipedia) repository to a new Git repository.It attempts to construct meaningful changesets (i.e.Git commits) based on chronologically grouping individual project/ file revisions.");
            sb.AppendLine("");
            sb.AppendLine(string.Format("Example: {0} -a \"c:\\temp\\vss.tools\" -b \"$/MainEnv/Tool1\" -d \"c:\\temp\\output\" --PathRegex=\".*\" --EmailDomain=example.com -j True -k True -m 31 -n 601 -x", AppDomain.CurrentDomain.FriendlyName));
            sb.AppendLine("");
            sb.AppendLine("Options:");
            sb.AppendLine(fullString);
            this.helpText.Text = sb.ToString();
                        
            this.helpText.Select(0, 0);
            this.ShowDialog();
        }

    }
}
