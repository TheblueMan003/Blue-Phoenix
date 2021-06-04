namespace JSharp
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.datapackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newDatapackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reformatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findAndReplaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compileOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.minecraftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.structuresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.libraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getCallStackTraceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateResourcesPackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProjectSave = new System.Windows.Forms.SaveFileDialog();
            this.ProjectOpen = new System.Windows.Forms.OpenFileDialog();
            this.ExportSave = new System.Windows.Forms.SaveFileDialog();
            this.ErrorBox = new System.Windows.Forms.RichTextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.DatapackOpen = new System.Windows.Forms.OpenFileDialog();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.ExportRP = new System.Windows.Forms.SaveFileDialog();
            this.button15 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.button16 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.LibraryButton = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.CodeListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.folderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.SaveButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ShowInfoButton = new System.Windows.Forms.Button();
            this.ShowWarningButton = new System.Windows.Forms.Button();
            this.ShowErrorButton = new System.Windows.Forms.Button();
            this.ClearLogButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.CodeBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.autocompleteMenu1 = new AutocompleteMenuNS.AutocompleteMenu();
            this.menuStrip1.SuspendLayout();
            this.CodeListContextMenu.SuspendLayout();
            this.tabControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CodeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.displayToolStripMenuItem,
            this.projectToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1192, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.openToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.newProjectToolStripMenuItem.Text = "New Project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.saveAsToolStripMenuItem.Text = "Save as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.datapackToolStripMenuItem,
            this.newDatapackToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // datapackToolStripMenuItem
            // 
            this.datapackToolStripMenuItem.Name = "datapackToolStripMenuItem";
            this.datapackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.datapackToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.datapackToolStripMenuItem.Text = "Datapack";
            this.datapackToolStripMenuItem.Click += new System.EventHandler(this.datapackToolStripMenuItem_Click);
            // 
            // newDatapackToolStripMenuItem
            // 
            this.newDatapackToolStripMenuItem.Name = "newDatapackToolStripMenuItem";
            this.newDatapackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.E)));
            this.newDatapackToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.newDatapackToolStripMenuItem.Text = "New Datapack";
            this.newDatapackToolStripMenuItem.Click += new System.EventHandler(this.newDatapackToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // displayToolStripMenuItem
            // 
            this.displayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reformatToolStripMenuItem,
            this.findToolStripMenuItem,
            this.findAndReplaceToolStripMenuItem,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
            this.displayToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            this.displayToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.displayToolStripMenuItem.Text = "Edit";
            // 
            // reformatToolStripMenuItem
            // 
            this.reformatToolStripMenuItem.Name = "reformatToolStripMenuItem";
            this.reformatToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.reformatToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.reformatToolStripMenuItem.Text = "Reformat";
            this.reformatToolStripMenuItem.Click += new System.EventHandler(this.reformatToolStripMenuItem_Click_1);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.findToolStripMenuItem.Text = "Find";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // findAndReplaceToolStripMenuItem
            // 
            this.findAndReplaceToolStripMenuItem.Name = "findAndReplaceToolStripMenuItem";
            this.findAndReplaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.findAndReplaceToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.findAndReplaceToolStripMenuItem.Text = "Find and Replace";
            this.findAndReplaceToolStripMenuItem.Click += new System.EventHandler(this.findAndReplaceToolStripMenuItem_Click);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFileToolStripMenuItem,
            this.compileToolStripMenuItem,
            this.compileOrderToolStripMenuItem,
            this.tagsToolStripMenuItem,
            this.structuresToolStripMenuItem,
            this.libraryToolStripMenuItem,
            this.settingToolStripMenuItem});
            this.projectToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.projectToolStripMenuItem.Text = "Project";
            // 
            // newFileToolStripMenuItem
            // 
            this.newFileToolStripMenuItem.Name = "newFileToolStripMenuItem";
            this.newFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newFileToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.newFileToolStripMenuItem.Text = "New File";
            this.newFileToolStripMenuItem.Click += new System.EventHandler(this.newFileToolStripMenuItem_Click);
            // 
            // compileToolStripMenuItem
            // 
            this.compileToolStripMenuItem.Name = "compileToolStripMenuItem";
            this.compileToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.compileToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.compileToolStripMenuItem.Text = "Compile";
            this.compileToolStripMenuItem.Click += new System.EventHandler(this.compileToolStripMenuItem_Click);
            // 
            // compileOrderToolStripMenuItem
            // 
            this.compileOrderToolStripMenuItem.Name = "compileOrderToolStripMenuItem";
            this.compileOrderToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.compileOrderToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.compileOrderToolStripMenuItem.Text = "Compile Order";
            this.compileOrderToolStripMenuItem.Click += new System.EventHandler(this.compileOrderToolStripMenuItem_Click);
            // 
            // tagsToolStripMenuItem
            // 
            this.tagsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem1,
            this.minecraftToolStripMenuItem});
            this.tagsToolStripMenuItem.Name = "tagsToolStripMenuItem";
            this.tagsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.tagsToolStripMenuItem.Text = "Tags";
            this.tagsToolStripMenuItem.Click += new System.EventHandler(this.tagsToolStripMenuItem_Click);
            // 
            // projectToolStripMenuItem1
            // 
            this.projectToolStripMenuItem1.Name = "projectToolStripMenuItem1";
            this.projectToolStripMenuItem1.Size = new System.Drawing.Size(125, 22);
            this.projectToolStripMenuItem1.Text = "Project";
            this.projectToolStripMenuItem1.Click += new System.EventHandler(this.projectToolStripMenuItem1_Click);
            // 
            // minecraftToolStripMenuItem
            // 
            this.minecraftToolStripMenuItem.Name = "minecraftToolStripMenuItem";
            this.minecraftToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.minecraftToolStripMenuItem.Text = "Minecraft";
            this.minecraftToolStripMenuItem.Click += new System.EventHandler(this.minecraftToolStripMenuItem_Click);
            // 
            // structuresToolStripMenuItem
            // 
            this.structuresToolStripMenuItem.Name = "structuresToolStripMenuItem";
            this.structuresToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.structuresToolStripMenuItem.Text = "Structures";
            this.structuresToolStripMenuItem.Click += new System.EventHandler(this.structuresToolStripMenuItem_Click);
            // 
            // libraryToolStripMenuItem
            // 
            this.libraryToolStripMenuItem.Name = "libraryToolStripMenuItem";
            this.libraryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.libraryToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.libraryToolStripMenuItem.Text = "Library";
            this.libraryToolStripMenuItem.Click += new System.EventHandler(this.libraryToolStripMenuItem_Click);
            // 
            // settingToolStripMenuItem
            // 
            this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
            this.settingToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.settingToolStripMenuItem.Text = "Setting";
            this.settingToolStripMenuItem.Click += new System.EventHandler(this.settingToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getCallStackTraceToolStripMenuItem,
            this.generateResourcesPackToolStripMenuItem});
            this.toolsToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // getCallStackTraceToolStripMenuItem
            // 
            this.getCallStackTraceToolStripMenuItem.Name = "getCallStackTraceToolStripMenuItem";
            this.getCallStackTraceToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.getCallStackTraceToolStripMenuItem.Text = "Get Call Stack Trace";
            this.getCallStackTraceToolStripMenuItem.Click += new System.EventHandler(this.getCallStackTraceToolStripMenuItem_Click);
            // 
            // generateResourcesPackToolStripMenuItem
            // 
            this.generateResourcesPackToolStripMenuItem.Name = "generateResourcesPackToolStripMenuItem";
            this.generateResourcesPackToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.generateResourcesPackToolStripMenuItem.Text = "Generate Resources Pack";
            this.generateResourcesPackToolStripMenuItem.Click += new System.EventHandler(this.generateResourcesPackToolStripMenuItem_Click);
            // 
            // ProjectSave
            // 
            this.ProjectSave.Filter = "*.tbms|*.tbms";
            this.ProjectSave.Title = "Save Project";
            // 
            // ProjectOpen
            // 
            this.ProjectOpen.Filter = "*.tbms|*.tbms";
            this.ProjectOpen.Title = "OpenProject";
            // 
            // ErrorBox
            // 
            this.ErrorBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.autocompleteMenu1.SetAutocompleteMenu(this.ErrorBox, null);
            this.ErrorBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ErrorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ErrorBox.ForeColor = System.Drawing.Color.White;
            this.ErrorBox.Location = new System.Drawing.Point(237, 563);
            this.ErrorBox.Name = "ErrorBox";
            this.ErrorBox.ReadOnly = true;
            this.ErrorBox.Size = new System.Drawing.Size(909, 164);
            this.ErrorBox.TabIndex = 1;
            this.ErrorBox.Text = "";
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.BackColor = System.Drawing.Color.Black;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button5.Location = new System.Drawing.Point(1038, 199);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(142, 23);
            this.button5.TabIndex = 9;
            this.button5.Text = "Functions";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.FunctionPreview_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.BackColor = System.Drawing.Color.Black;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button3.Location = new System.Drawing.Point(1038, 109);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(142, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "Gamerule";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.GameruleGenerator_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.BackColor = System.Drawing.Color.Black;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button4.Location = new System.Drawing.Point(1038, 80);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(142, 23);
            this.button4.TabIndex = 7;
            this.button4.Text = "Effect";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.EffectGenerator_Click);
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.BackColor = System.Drawing.Color.Black;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button6.Location = new System.Drawing.Point(1038, 257);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(142, 23);
            this.button6.TabIndex = 13;
            this.button6.Text = "Structures";
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.StructPreview_Click);
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button7.BackColor = System.Drawing.Color.Black;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button7.Location = new System.Drawing.Point(1038, 170);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(142, 23);
            this.button7.TabIndex = 14;
            this.button7.Text = "Variables";
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Click += new System.EventHandler(this.VariablePreview_Click);
            // 
            // button8
            // 
            this.button8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button8.BackColor = System.Drawing.Color.Black;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button8.Location = new System.Drawing.Point(1038, 228);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(142, 23);
            this.button8.TabIndex = 15;
            this.button8.Text = "Enums";
            this.button8.UseVisualStyleBackColor = false;
            this.button8.Click += new System.EventHandler(this.EnumPreview_Click);
            // 
            // button9
            // 
            this.button9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button9.BackColor = System.Drawing.Color.Black;
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button9.Location = new System.Drawing.Point(1038, 522);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(142, 23);
            this.button9.TabIndex = 16;
            this.button9.Text = "Sounds";
            this.button9.UseVisualStyleBackColor = false;
            this.button9.Click += new System.EventHandler(this.SoundPreview_Click);
            // 
            // button10
            // 
            this.button10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button10.BackColor = System.Drawing.Color.Black;
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button10.Location = new System.Drawing.Point(1038, 493);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(142, 23);
            this.button10.TabIndex = 17;
            this.button10.Text = "Block/Item";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.BlockPreview_Click);
            // 
            // button12
            // 
            this.button12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button12.BackColor = System.Drawing.Color.Black;
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button12.Location = new System.Drawing.Point(1038, 359);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(142, 23);
            this.button12.TabIndex = 22;
            this.button12.Text = "Predicates";
            this.button12.UseVisualStyleBackColor = false;
            this.button12.Click += new System.EventHandler(this.PredicatePreview_Click);
            // 
            // button13
            // 
            this.button13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button13.BackColor = System.Drawing.Color.Black;
            this.button13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button13.Location = new System.Drawing.Point(1038, 286);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(142, 23);
            this.button13.TabIndex = 23;
            this.button13.Text = "Class";
            this.button13.UseVisualStyleBackColor = false;
            this.button13.Click += new System.EventHandler(this.ClassPreview_Click);
            // 
            // button14
            // 
            this.button14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button14.BackColor = System.Drawing.Color.Black;
            this.button14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button14.Location = new System.Drawing.Point(1038, 388);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(142, 23);
            this.button14.TabIndex = 24;
            this.button14.Text = "Blocktags";
            this.button14.UseVisualStyleBackColor = false;
            this.button14.Click += new System.EventHandler(this.BlockTagsPreview_Click);
            // 
            // ExportRP
            // 
            this.ExportRP.Filter = "*.zip|*.zip";
            // 
            // button15
            // 
            this.button15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button15.BackColor = System.Drawing.Color.Black;
            this.button15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button15.Location = new System.Drawing.Point(1038, 417);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(142, 23);
            this.button15.TabIndex = 25;
            this.button15.Text = "Entitytags";
            this.button15.UseVisualStyleBackColor = false;
            this.button15.Click += new System.EventHandler(this.EntityTagsPreview_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(1038, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Inspector:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1038, 154);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "TBMS:";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1038, 343);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 28;
            this.label5.Text = "JSON Elements:";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1038, 477);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 29;
            this.label6.Text = "MC List:";
            // 
            // button16
            // 
            this.button16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button16.BackColor = System.Drawing.Color.Black;
            this.button16.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.button16.Location = new System.Drawing.Point(1038, 446);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(142, 23);
            this.button16.TabIndex = 30;
            this.button16.Text = "Itemtags";
            this.button16.UseVisualStyleBackColor = false;
            this.button16.Click += new System.EventHandler(this.CompileJava_Click);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(1038, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "Generator:";
            // 
            // LibraryButton
            // 
            this.LibraryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LibraryButton.BackColor = System.Drawing.Color.Black;
            this.LibraryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LibraryButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.LibraryButton.Location = new System.Drawing.Point(1038, 315);
            this.LibraryButton.Name = "LibraryButton";
            this.LibraryButton.Size = new System.Drawing.Size(142, 23);
            this.LibraryButton.TabIndex = 32;
            this.LibraryButton.Text = "Library";
            this.LibraryButton.UseVisualStyleBackColor = false;
            this.LibraryButton.Click += new System.EventHandler(this.LibraryButton_Click);
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.treeView1.ContextMenuStrip = this.CodeListContextMenu;
            this.treeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.treeView1.ForeColor = System.Drawing.Color.White;
            this.treeView1.FullRowSelect = true;
            this.treeView1.Indent = 10;
            this.treeView1.Location = new System.Drawing.Point(12, 62);
            this.treeView1.Name = "treeView1";
            this.treeView1.PathSeparator = "/";
            this.treeView1.ShowLines = false;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(219, 665);
            this.treeView1.TabIndex = 33;
            this.treeView1.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCollapse);
            this.treeView1.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterExpand);
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // CodeListContextMenu
            // 
            this.CodeListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFileToolStripMenuItem1,
            this.deleteToolStripMenuItem,
            this.newFolderToolStripMenuItem});
            this.CodeListContextMenu.Name = "contextMenuStrip1";
            this.CodeListContextMenu.Size = new System.Drawing.Size(156, 70);
            // 
            // newFileToolStripMenuItem1
            // 
            this.newFileToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.folderToolStripMenuItem});
            this.newFileToolStripMenuItem1.Name = "newFileToolStripMenuItem1";
            this.newFileToolStripMenuItem1.Size = new System.Drawing.Size(155, 22);
            this.newFileToolStripMenuItem1.Text = "New";
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.fileToolStripMenuItem1.Text = "File";
            this.fileToolStripMenuItem1.Click += new System.EventHandler(this.fileToolStripMenuItem1_Click);
            // 
            // folderToolStripMenuItem
            // 
            this.folderToolStripMenuItem.Name = "folderToolStripMenuItem";
            this.folderToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.folderToolStripMenuItem.Text = "Folder";
            this.folderToolStripMenuItem.Click += new System.EventHandler(this.folderToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.animatedToolStripMenuItem});
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.newFolderToolStripMenuItem.Text = "Resources Pack";
            // 
            // animatedToolStripMenuItem
            // 
            this.animatedToolStripMenuItem.Name = "animatedToolStripMenuItem";
            this.animatedToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.animatedToolStripMenuItem.Text = "Animated";
            this.animatedToolStripMenuItem.Click += new System.EventHandler(this.animatedToolStripMenuItem_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.ForeColor = System.Drawing.Color.Black;
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(787, 0);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(237, 42);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(795, 19);
            this.tabControl1.TabIndex = 40;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // SaveButton
            // 
            this.SaveButton.BackColor = System.Drawing.Color.Black;
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.ForeColor = System.Drawing.Color.White;
            this.SaveButton.Image = global::BluePhoenix.Properties.Resources.save;
            this.SaveButton.Location = new System.Drawing.Point(46, 31);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(28, 28);
            this.SaveButton.TabIndex = 41;
            this.SaveButton.UseVisualStyleBackColor = false;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(237, 62);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(795, 492);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 38;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // ShowInfoButton
            // 
            this.ShowInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowInfoButton.BackColor = System.Drawing.Color.Black;
            this.ShowInfoButton.FlatAppearance.BorderSize = 3;
            this.ShowInfoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ShowInfoButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.ShowInfoButton.Image = global::BluePhoenix.Properties.Resources.info;
            this.ShowInfoButton.Location = new System.Drawing.Point(1152, 656);
            this.ShowInfoButton.Name = "ShowInfoButton";
            this.ShowInfoButton.Size = new System.Drawing.Size(28, 28);
            this.ShowInfoButton.TabIndex = 37;
            this.ShowInfoButton.UseVisualStyleBackColor = false;
            this.ShowInfoButton.Click += new System.EventHandler(this.InfoButton_Click);
            // 
            // ShowWarningButton
            // 
            this.ShowWarningButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowWarningButton.BackColor = System.Drawing.Color.Black;
            this.ShowWarningButton.FlatAppearance.BorderSize = 3;
            this.ShowWarningButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ShowWarningButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.ShowWarningButton.Image = global::BluePhoenix.Properties.Resources.warning;
            this.ShowWarningButton.Location = new System.Drawing.Point(1152, 625);
            this.ShowWarningButton.Name = "ShowWarningButton";
            this.ShowWarningButton.Size = new System.Drawing.Size(28, 28);
            this.ShowWarningButton.TabIndex = 36;
            this.ShowWarningButton.UseVisualStyleBackColor = false;
            this.ShowWarningButton.Click += new System.EventHandler(this.WarningButton_Click);
            // 
            // ShowErrorButton
            // 
            this.ShowErrorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowErrorButton.BackColor = System.Drawing.Color.Black;
            this.ShowErrorButton.FlatAppearance.BorderSize = 3;
            this.ShowErrorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ShowErrorButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.ShowErrorButton.Image = global::BluePhoenix.Properties.Resources.error;
            this.ShowErrorButton.Location = new System.Drawing.Point(1152, 594);
            this.ShowErrorButton.Name = "ShowErrorButton";
            this.ShowErrorButton.Size = new System.Drawing.Size(28, 28);
            this.ShowErrorButton.TabIndex = 35;
            this.ShowErrorButton.UseVisualStyleBackColor = false;
            this.ShowErrorButton.Click += new System.EventHandler(this.ErrorButton_Click);
            // 
            // ClearLogButton
            // 
            this.ClearLogButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearLogButton.BackColor = System.Drawing.Color.Black;
            this.ClearLogButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ClearLogButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.ClearLogButton.Image = global::BluePhoenix.Properties.Resources.clear;
            this.ClearLogButton.Location = new System.Drawing.Point(1152, 563);
            this.ClearLogButton.Name = "ClearLogButton";
            this.ClearLogButton.Size = new System.Drawing.Size(28, 28);
            this.ClearLogButton.TabIndex = 34;
            this.ClearLogButton.UseVisualStyleBackColor = false;
            this.ClearLogButton.Click += new System.EventHandler(this.ClearLogButton_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Black;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Image = global::BluePhoenix.Properties.Resources.new_file;
            this.button2.Location = new System.Drawing.Point(12, 31);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(28, 28);
            this.button2.TabIndex = 6;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.Lime;
            this.button1.Image = global::BluePhoenix.Properties.Resources.run;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(1038, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(142, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Compile";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CodeBox
            // 
            this.CodeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CodeBox.AutoCompleteBrackets = true;
            this.CodeBox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.autocompleteMenu1.SetAutocompleteMenu(this.CodeBox, this.autocompleteMenu1);
            this.CodeBox.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.CodeBox.BackBrush = null;
            this.CodeBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CodeBox.CaretColor = System.Drawing.Color.White;
            this.CodeBox.CharHeight = 14;
            this.CodeBox.CharWidth = 8;
            this.CodeBox.CurrentLineColor = System.Drawing.Color.DimGray;
            this.CodeBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.CodeBox.DescriptionFile = "";
            this.CodeBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.CodeBox.FoldingIndicatorColor = System.Drawing.Color.Chartreuse;
            this.CodeBox.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.CodeBox.IndentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CodeBox.IsReplaceMode = false;
            this.CodeBox.LineNumberColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(255)))));
            this.CodeBox.Location = new System.Drawing.Point(237, 62);
            this.CodeBox.Name = "CodeBox";
            this.CodeBox.Paddings = new System.Windows.Forms.Padding(0);
            this.CodeBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.CodeBox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("CodeBox.ServiceColors")));
            this.CodeBox.ServiceLinesColor = System.Drawing.Color.DimGray;
            this.CodeBox.ShowFoldingLines = true;
            this.CodeBox.Size = new System.Drawing.Size(795, 492);
            this.CodeBox.TabIndex = 42;
            this.CodeBox.Zoom = 100;
            this.CodeBox.Load += new System.EventHandler(this.CodeBox_Load);
            this.CodeBox.Enter += new System.EventHandler(this.fastColoredTextBox1_Enter);
            // 
            // autocompleteMenu1
            // 
            this.autocompleteMenu1.AllowsTabKey = true;
            this.autocompleteMenu1.AppearInterval = 100;
            this.autocompleteMenu1.Colors = ((AutocompleteMenuNS.Colors)(resources.GetObject("autocompleteMenu1.Colors")));
            this.autocompleteMenu1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.autocompleteMenu1.ImageList = null;
            this.autocompleteMenu1.Items = new string[] {
        "public void ^ (){\\n\\n}",
        "public class ^ (){\\n\\n}",
        "public struct ^ (){\\n\\n}",
        "public enum ^{\\n\\n}"};
            this.autocompleteMenu1.MaximumSize = new System.Drawing.Size(300, 200);
            this.autocompleteMenu1.SearchPattern = "[\\$\\w\\.\\:\\@]";
            this.autocompleteMenu1.TargetControlWrapper = null;
            this.autocompleteMenu1.Selected += new System.EventHandler<AutocompleteMenuNS.SelectedEventArgs>(this.autocompleteMenu1_Selected);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.ClientSize = new System.Drawing.Size(1192, 732);
            this.Controls.Add(this.CodeBox);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ShowInfoButton);
            this.Controls.Add(this.ShowWarningButton);
            this.Controls.Add(this.ShowErrorButton);
            this.Controls.Add(this.ClearLogButton);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.LibraryButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button16);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.ErrorBox);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Default";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.CodeListContextMenu.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CodeBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog ProjectSave;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFileToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog ProjectOpen;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem datapackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reformatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compileOrderToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog ExportSave;
        private System.Windows.Forms.ToolStripMenuItem tagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem minecraftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newDatapackToolStripMenuItem;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.RichTextBox ErrorBox;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findAndReplaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.ToolStripMenuItem settingToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog DatapackOpen;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getCallStackTraceToolStripMenuItem;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.SaveFileDialog ExportRP;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.ToolStripMenuItem compileToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button LibraryButton;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button ClearLogButton;
        private System.Windows.Forms.Button ShowErrorButton;
        private System.Windows.Forms.Button ShowWarningButton;
        private System.Windows.Forms.Button ShowInfoButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem generateResourcesPackToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip CodeListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem newFileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem folderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem animatedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.ToolStripMenuItem structuresToolStripMenuItem;
        private FastColoredTextBoxNS.FastColoredTextBox CodeBox;
        private AutocompleteMenuNS.AutocompleteMenu autocompleteMenu1;
        private System.Windows.Forms.ToolStripMenuItem libraryToolStripMenuItem;
    }
}

