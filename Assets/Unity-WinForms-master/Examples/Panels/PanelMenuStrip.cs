namespace UnityWinForms.Examples.Panels
{
    using System.Drawing;
    using System.Windows.Forms;

    public class PanelMenuStrip : BaseExamplePanel
    {
        public override void Initialize()
        {
            var itemFile_New = new ToolStripMenuItem("New");
            var itemFile_Open = new ToolStripMenuItem("Open");
            var itemFile_OpenSer = new ToolStripMenuItem("Open From Server");
            var itemFile_Save = new ToolStripMenuItem("Save");
            var itemFile_SaveAs = new ToolStripMenuItem("SaveAs..");
            var itemFile_SaveSer = new ToolStripMenuItem("Save To Server");
            var itemFile_Exit = new ToolStripMenuItem("Exit");

            itemFile_New.Click += ItemFile_Click;
            itemFile_Open.Click += ItemFile_Click;
            itemFile_OpenSer.Click += ItemFile_Click;
            itemFile_Save.Click += ItemFile_Click;
            itemFile_SaveAs.Click += ItemFile_Click;
            itemFile_SaveSer.Click += ItemFile_Click;
            itemFile_Exit.Click += ItemFile_Click;

            itemFile_New.ShortcutKeys = Keys.Control | Keys.N;
            itemFile_Save.ShortcutKeys = Keys.Control | Keys.S;
            itemFile_SaveAs.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            itemFile_Exit.ShortcutKeys = Keys.Control | Keys.W;
            itemFile_Open.ShortcutKeys = Keys.Control | Keys.O;
            
            itemFile_Exit.Image = uwfAppOwner.Resources.Close;
            itemFile_Exit.uwfImageColor = Color.FromArgb(64, 64, 64);

            var itemEdit_Cut = new ToolStripMenuItem("Cut");
            var itemEdit_Copy = new ToolStripMenuItem("Copy");
            var itemEdit_Paste = new ToolStripMenuItem("Paste");

            itemEdit_Cut.Click += ItemFile_Click;
            itemEdit_Copy.Click += ItemFile_Click;
            itemEdit_Paste.Click += ItemFile_Click;


            itemEdit_Cut.ShortcutKeys = Keys.Control | Keys.X;
            itemEdit_Copy.ShortcutKeys = Keys.Control | Keys.C;
            itemEdit_Paste.ShortcutKeys = Keys.Control | Keys.V;


            var itemView_Zoomin = new ToolStripMenuItem("Zoom In");
            var itemView_FitScreen = new ToolStripMenuItem("Fit Screen");
            var itemView_Zoomout = new ToolStripMenuItem("Zoom Out");
            var itemView_FullScreen = new ToolStripMenuItem("FullScreen");

            itemView_Zoomin.Click += ItemFile_Click;
            itemView_FitScreen.Click += ItemFile_Click;
            itemView_Zoomout.Click += ItemFile_Click;
            itemView_FullScreen.Click += ItemFile_Click;


            itemView_Zoomin.ShortcutKeys = Keys.Control | Keys.Add;
            itemView_Zoomout.ShortcutKeys = Keys.Control | Keys.Subtract;
            itemView_FitScreen.ShortcutKeys = Keys.Control | Keys.F;
            itemView_FullScreen.ShortcutKeys = Keys.F11;

            var itemRun_Play = new ToolStripMenuItem("Play");
            var itemRun_Stop = new ToolStripMenuItem("Stop");

            var itemRun_Export = new ToolStripMenuItem("Export");
            var itemRun_Report = new ToolStripMenuItem("Genrate Report");

            itemRun_Play.Click += ItemFile_Click;
            itemRun_Stop.Click += ItemFile_Click;
            itemRun_Export.Click += ItemFile_Click;
            itemRun_Report.Click += ItemFile_Click;


            itemRun_Play.ShortcutKeys = Keys.F5;
            itemRun_Stop.ShortcutKeys = Keys.F6;
            itemRun_Export.ShortcutKeys = Keys.Control | Keys.E;
            itemRun_Report.ShortcutKeys = Keys.Control | Keys.R;

            var itemGame_Play = new ToolStripMenuItem("Play Game");
            var itemGame_Stop = new ToolStripMenuItem("Stop Game");
            var itemGame_Next = new ToolStripMenuItem("Next");
            var itemGame_Prev = new ToolStripMenuItem("Previous");

            itemGame_Play.Click += ItemFile_Click;
            itemGame_Stop.Click += ItemFile_Click;
            itemGame_Next.Click += ItemFile_Click;
            itemGame_Prev.Click += ItemFile_Click;


            itemGame_Play.ShortcutKeys = Keys.F2;
            itemGame_Stop.ShortcutKeys = Keys.F3;
            itemGame_Next.ShortcutKeys = Keys.Up;
            itemGame_Prev.ShortcutKeys = Keys.Down;

            var itemFile = new ToolStripMenuItem("File");
            var itemEdit = new ToolStripMenuItem("Edit");
            var itemView = new ToolStripMenuItem("View");
            var itemRun = new ToolStripMenuItem("Run");
            var itemReport = new ToolStripMenuItem("Report");
            var itemGame = new ToolStripMenuItem("Game Mode");

            var itemTut = new ToolStripMenuItem("Tutorial");
            var itemTut_Play = new ToolStripMenuItem("Start Tutorial");
            var itemTut_Stop = new ToolStripMenuItem("Stop Tutorial");

            itemTut_Play.Click += ItemFile_Click;
            itemTut_Stop.Click += ItemFile_Click;

            itemTut.DropDownItems.Add(itemTut_Play);
            itemTut.DropDownItems.Add(itemTut_Stop);


            itemFile.DropDownItems.Add(itemFile_New);
            itemFile.DropDownItems.Add(itemFile_Open);
            itemFile.DropDownItems.Add(itemFile_OpenSer);
            itemFile.DropDownItems.Add(itemFile_Save);
            itemFile.DropDownItems.Add(itemFile_SaveAs);
            itemFile.DropDownItems.Add(itemFile_SaveSer);
            itemFile.DropDownItems.Add(new ToolStripSeparator());
            itemFile.DropDownItems.Add(itemFile_Exit);

            itemEdit.DropDownItems.Add(itemEdit_Cut);
            itemEdit.DropDownItems.Add(itemEdit_Copy);
            itemEdit.DropDownItems.Add(itemEdit_Paste);

            itemView.DropDownItems.Add(itemView_Zoomin);
            itemView.DropDownItems.Add(itemView_FitScreen);
            itemView.DropDownItems.Add(itemView_Zoomout);
            itemView.DropDownItems.Add(new ToolStripSeparator());
            itemView.DropDownItems.Add(itemView_FullScreen);

            itemRun.DropDownItems.Add(itemRun_Play);
            itemRun.DropDownItems.Add(itemRun_Stop);
            itemRun.DropDownItems.Add(new ToolStripSeparator());
            
            itemReport.DropDownItems.Add(itemRun_Export);
            itemReport.DropDownItems.Add(itemRun_Report);

            itemGame.DropDownItems.Add(itemGame_Play);
            itemGame.DropDownItems.Add(itemGame_Stop);
            itemGame.DropDownItems.Add(new ToolStripSeparator());
            itemGame.DropDownItems.Add(itemGame_Next);
            itemGame.DropDownItems.Add(itemGame_Prev);

            var menu = new MenuStrip();
            menu.Items.Add(itemFile);
            menu.Items.Add(itemEdit);
            menu.Items.Add(itemView);
            menu.Items.Add(itemRun);
            menu.Items.Add(itemReport);
            menu.Items.Add(itemGame);
            menu.Items.Add(itemTut);
            Controls.Add(menu);
        }

        private void ItemFile_Click(object sender, System.EventArgs e)
        {
            Manager.Instance.GetBymenu(sender.ToString());
        }
    }
}
