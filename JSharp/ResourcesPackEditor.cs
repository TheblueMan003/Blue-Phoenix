using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BluePhoenix
{
    public partial class ResourcesPackEditor : Form
    {
        public string path;
        public string parentPath;
        public string currentFile;
        public FileType currentFileType;
        public FolderDisplay folderDisplay;

        public ResourcesPackEditor(string path)
        {
            this.path = path;
            parentPath = Directory.GetParent(path).FullName;
            InitializeComponent();
            Reload();
        }

        private void ResourcesPackEditor_Load(object sender, EventArgs e)
        {

        }

        private void Reload()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            treeView1.Nodes.Clear();
            BuildTree(directoryInfo, treeView1.Nodes);
        }

        private void BuildTree(DirectoryInfo directoryInfo, TreeNodeCollection addInMe)
        {
            TreeNode curNode = addInMe.Add(directoryInfo.Name);

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                curNode.Nodes.Add(file.FullName, file.Name);
            }
            foreach (DirectoryInfo subdir in directoryInfo.GetDirectories())
            {
                BuildTree(subdir, curNode.Nodes);
            }
        }

        public void Select(FileType type, string path)
        {
            CodeBox.Visible = false;
            CodeBox.Enabled = false;
            pictureBox1.Visible = false;
            pictureBox1.Enabled = false;
            panel1.Visible = false;
            panel1.Enabled = false;

            currentFile = path;
            currentFileType = type;

            if (type == FileType.Text)
            {
                CodeBox.Visible = true;
                CodeBox.Enabled = true;
                try
                {
                    CodeBox.Text = File.ReadAllText(path);
                }
                catch
                {
                    CodeBox.Text = "Unreadable Shit";
                    CodeBox.Enabled = false;
                }
            }
            if (type == FileType.Image)
            {
                pictureBox1.Visible = true;
                pictureBox1.Enabled = true;
                pictureBox1.Image = Image.FromFile(path);
            }
            if (type == FileType.Directory)
            {
                panel1.Visible = true;
                panel1.Enabled = true;

                if (folderDisplay == null)
                    folderDisplay = new FolderDisplay(panel1, FileMenu, DirectoryMenu, vScrollBar1, ImagePreview);
                folderDisplay.LoadFolder(new Folder(path));
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string fName = treeView1.SelectedNode.Name;
            string fullPath = treeView1.SelectedNode.FullPath;
            
            if (fName.EndsWith(".jpeg") || fName.EndsWith(".png"))
            {
                Select(FileType.Image, fName);
            }
            else if (fName.EndsWith(".ogg") || fName.EndsWith(".ogg"))
            {
                Select(FileType.Sound, fName);
            }
            else if (fName.EndsWith(".txt") || fName.EndsWith(".json") || fName.EndsWith(".mcmeta"))
            {
                Select(FileType.Text, fName);
            }
            else if (Directory.Exists(parentPath + "/"+ fullPath))
            {
                Select(FileType.Directory, parentPath + "/" + fullPath);
            }
            Text = parentPath + "/" + fullPath;
        }
        public enum FileType
        {
            Directory,
            Sound,
            Image,
            Text
        }

        private void ResourcesPackEditor_Activated(object sender, EventArgs e)
        {
            Reload();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (currentFileType == FileType.Image)
            {
                System.Diagnostics.Process.Start("\"C:\\Program Files\\Paint.NET\\PaintDotNet.exe\"",currentFile);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (folderDisplay != null)
                folderDisplay.Draw(panel1);
        }
    }

    public interface IDragable
    {
        void StartDrag();
        void EndDrag();
        void SetOwner(IDragableOwner dragableOwner);
        void Draw(Control container);
        bool IsDragged();
    }
    public interface IDragableOwner
    {
        int GetW();
        int GetH();

        void Update();
    }
    public enum Constraints
    {
        None,
        Horizontal,
        Vertical,
        All
    }
    public abstract class AbstractDragable : IDragable
    {
        private bool isDraged = false;
        private IDragableOwner dragableOwner;

        private int x;
        private int y;
        private int w;
        private int h;

        private int sx;
        private int sy;

        public virtual void Draw(Control container)
        {
            if (isDraged)
            {
                x += Cursor.Position.X - sx;
                y += Cursor.Position.Y - sy;

                sx = Cursor.Position.X;
                sy = Cursor.Position.Y;
            }
        }

        public virtual void EndDrag()
        {
            isDraged = false;

            Constraints constraint = GetConstraints();

            if (constraint == Constraints.All || constraint == Constraints.Horizontal)
            {
                x = 0;
            }

            if (constraint == Constraints.All || constraint == Constraints.Vertical)
            {
                y = 0;
            }

            dragableOwner.Update();
        }

        public abstract Constraints GetConstraints();

        public virtual void SetOwner(IDragableOwner dragableOwner)
        {
            this.dragableOwner = dragableOwner;
        }

        public virtual void StartDrag()
        {
            isDraged = true;
            sx = Cursor.Position.X;
            sy = Cursor.Position.Y;
        }

        public virtual void SetSize(int w, int h)
        {
            this.w = w;
            this.h = h;
        }

        public virtual void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int GetX() => x;
        public int GetY() => y;
        public int GetW() => w;
        public int GetH() => h;

        public virtual void ClickDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                StartDrag();
            }
        }

        public virtual void ClickUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                EndDrag();
            }
        }

        public bool IsDragged()
        {
            return isDraged;
        }
    }
    public abstract class FolderItem
    {
        public virtual string path { get; private set; }
        public virtual string name { get; private set; }

        public FolderItem(string path)
        {
            this.path = path;
            this.name = path.Replace("/", "\\").Substring(path.LastIndexOf("\\") + 1, path.Length - (path.LastIndexOf("\\") + 1));

            if (name == "")
            {
                name = path;
            }
        }

        public abstract DateTime GetCreationDate();
        public abstract DateTime GetModificationDate();
        public abstract DateTime GetLateAccessDate();
        public abstract FolderItem GetParent();

        public abstract void MoveTo(string folder);
        public abstract void CopyTo(string folder);

        public bool isImage()
        {
            if (File.Exists(path) && FileFilter.IsImage(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class Folder : FolderItem
    {
        private FolderFile icon;
        public Folder(string folder) : base(folder)
        {

        }

        public virtual List<FolderItem> GetContent()
        {
            List<FolderItem> content = new List<FolderItem>();
            try
            {
                foreach (string folder in Directory.GetDirectories(path))
                {
                    content.Add(new Folder(folder));
                }
                foreach (string file in Directory.GetFiles(path))
                {
                    content.Add(new FolderFile(file));
                }
            }
            catch { }


            return content;
        }

        public override DateTime GetCreationDate()
        {
            return Directory.GetCreationTime(path);
        }

        public override DateTime GetModificationDate()
        {
            return Directory.GetLastWriteTime(path);
        }

        public override DateTime GetLateAccessDate()
        {
            return Directory.GetLastAccessTime(path);
        }

        public override FolderItem GetParent()
        {
            if (Directory.GetParent(path) != null)
                return new Folder(Directory.GetParent(path).FullName);
            else
                return this;
        }

        public bool HasIcon()
        {
            int img = 0;
            foreach (FolderItem f in GetContent())
            {
                if (f.isImage())
                {
                    img += 1;
                    icon = (FolderFile)f;
                }
            }
            return img == 1;
        }
        public FolderFile GetIcon()
        {
            return icon;
        }

        public override void MoveTo(string folder)
        {
            //throw new NotImplementedException();
        }

        public override void CopyTo(string folder)
        {
            //throw new NotImplementedException();
        }
    }
    public class FolderDisplay : IDragableOwner
    {
        private List<ItemDisplayer> strips = new List<ItemDisplayer>();
        public Control container;
        private ContextMenuStrip fileMenu;
        private ContextMenuStrip directoryMenu;
        private Folder folder;
        public bool dragDrop;
        private ScrollBar scrollbar;
        private int amount;
        private PictureBox preview;

        public FolderDisplay(Control container, ContextMenuStrip fileMenu, ContextMenuStrip directoryMenu, ScrollBar scrollbar, PictureBox preview)
        {
            this.container = container;
            this.fileMenu = fileMenu;
            this.directoryMenu = directoryMenu;
            this.scrollbar = scrollbar;
            this.preview = preview;
            //container.DoubleClick += new EventHandler(ClickDouble);
        }

        public ItemDisplayer AddEmptyItem(FolderItem file, ContextMenuStrip fileMenu, ContextMenuStrip directoryMenu, PictureBox preview)
        {
            ItemDisplayer strip = new ItemDisplayer(this, this, file, fileMenu, directoryMenu, preview);
            strip.SetOwner(this);
            strips.Add(strip);

            return strip;
        }
        public FolderDisplay AddItem(ItemDisplayer strip)
        {
            strip.SetOwner(this);
            strips.Add(strip);

            return this;
        }

        public FolderDisplay RemoveItem(ItemDisplayer strip)
        {
            strips.Remove(strip);
            return this;
        }

        public void Clear()
        {
            while (strips.Count > 0)
            {
                strips[0].Delete();
            }
        }

        public void Reload()
        {
            LoadFolder(folder, true);
        }

        public void LoadFolder(Folder folder, bool force = false)
        {
            Clear();
            this.folder = folder;

            amount = 0;
            foreach (FolderItem item in folder.GetContent())
            {
                AddEmptyItem(item, fileMenu, directoryMenu, preview);
                amount++;
            }
        }

        public void Draw(Control container)
        {
            foreach (ItemDisplayer strip in strips)
            {
                strip.Draw(container);
            }

            Update();
            /*
            foreach (ItemDisplayer strip in strips)
            {
                strip.Update();
            }*/
        }

        public int GetH()
        {
            return container.Size.Height;
        }

        public int GetW()
        {
            return container.Size.Width;
        }

        public void Delete()
        {
            Clear();
        }

        public void Update()
        {
            int y = 0;
            int x = 0;

            strips.Sort(new Comparer());
            int shift = scrollbar.Value;
            int max = 0;

            for (int i = 0; i < strips.Count; i++)
            {
                if (!strips[i].IsDragged())
                {
                    strips[i].SetPosition(x, y - shift);
                }
                x += strips[i].GetW() + 5;
                if (x > GetW() - strips[i].GetW())
                {
                    y += strips[i].GetH() + 5;
                    if (y > GetH())
                    {
                        max += 120;
                    }
                    x = 0;
                }
            }
            max += 120;
            if (max != scrollbar.Maximum)
            {
                scrollbar.LargeChange = 120;
                scrollbar.Maximum = max;
            }
        }

        public void UnselectedAll()
        {
            foreach (ItemDisplayer item in strips)
            {
                item.selected = false;
            }
        }

        public List<FolderFile> GetSelectedFiles()
        {
            List<FolderFile> lst = new List<FolderFile>();
            foreach (ItemDisplayer item in strips)
            {
                if (item.selected)
                {
                    if (item.file is FolderFile)
                        lst.Add((FolderFile)item.file);
                }
            }
            return lst;
        }

        public List<Folder> GetSelectedDirectory()
        {
            List<Folder> lst = new List<Folder>();
            foreach (ItemDisplayer item in strips)
            {
                if (item.selected)
                {
                    if (item.file is Folder)
                        lst.Add((Folder)item.file);
                }
            }
            return lst;
        }

        public int GetAmount()
        {
            return amount;
        }

        class Comparer : IComparer<ItemDisplayer>
        {
            public int Compare(ItemDisplayer x, ItemDisplayer y)
            {
                //return (x.GetX() - y.GetX()) + ((x.GetY() - y.GetY())/x.GetH())*5000;
                return string.Compare(x.file.name, y.file.name) + (x.isFolder ? -100000 : 0) + (y.isFolder ? 100000 : 0);
            }
        }
    }
    public class ItemDisplayer : AbstractDragable
    {
        private Panel panel;
        private Control container;
        private FolderDisplay folderDisplay;
        private Label label;
        public FolderItem file;
        private PictureBox iconBox;
        public bool selected;
        private ContextMenuStrip fileMenu;
        private ContextMenuStrip directoryMenu;
        private int clickTime;
        public bool isFolder;
        private PictureBox preview;
        public static int imageSize = 64;
        private static Dictionary<string, Image> GlobalIcons = new Dictionary<string, Image>();

        public ItemDisplayer(IDragableOwner dragableOwner, FolderDisplay folderDisplay, FolderItem file,
            ContextMenuStrip fileMenu, ContextMenuStrip directoryMenu, PictureBox preview)
        {
            SetPosition(0, 0);

            if (file is DriveFolder)
                SetSize(256, 96);
            else
                SetSize(64, 96);

            SetOwner(dragableOwner);

            this.folderDisplay = folderDisplay;
            this.file = file;
            this.fileMenu = fileMenu;
            this.preview = preview;
            this.directoryMenu = directoryMenu;
        }

        public override void Draw(Control container)
        {
            base.Draw(container);
            this.container = container;

            if (clickTime > 0)
                clickTime -= 1;

            if (panel == null)
            {
                panel = new Panel();
                container.Controls.Add(panel);

                panel.MouseDown += new MouseEventHandler(ClickDown);
                panel.MouseUp += new MouseEventHandler(ClickUp);
                panel.DoubleClick += new EventHandler(DoubleClick);

                //Label
                label = new Label();
                panel.Controls.Add(label);
                label.Location = new Point(0, GetH() - 32);
                label.Size = new Size(GetW(), 32);
                label.AutoSize = false;
                label.ForeColor = Color.White;
                label.TextAlign = ContentAlignment.TopCenter;
                label.AutoEllipsis = true;
                label.Text = file.name;

                //Icon
                iconBox = new PictureBox();
                panel.Controls.Add(iconBox);
                iconBox.Size = new Size(imageSize, imageSize);
                iconBox.DoubleClick += new EventHandler(DoubleClick);

                iconBox.MouseDown += new MouseEventHandler(ClickDown);
                iconBox.MouseUp += new MouseEventHandler(ClickUp);
                iconBox.SizeMode = PictureBoxSizeMode.StretchImage;
                label.BringToFront();

                //Bar
                if (file is DriveFolder)
                {
                    LoadDrive();
                }


                if (file is FolderFile)
                {
                    if (file.isImage())
                    {
                        try
                        {
                            iconBox.Image = Image.FromFile(file.path);
                        }
                        catch { }
                    }
                    else
                    {
                        string ext = ((FolderFile)file).GetExtension();
                        try
                        {
                            if ((!GlobalIcons.ContainsKey(ext)) || ext == ".exe")
                            {
                                Image img = Icon.ExtractAssociatedIcon(file.path).ToBitmap();
                                iconBox.Image = img;

                                if (ext != ".exe")
                                    GlobalIcons.Add(ext, img);
                            }
                            else
                            {
                                Image img = GlobalIcons[ext];
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    iconBox.Image = Icon.ExtractAssociatedIcon("C:\\Windows/explorer.exe").ToBitmap();
                    if (((Folder)file).HasIcon())
                    {
                        var subiconBox = new PictureBox();
                        iconBox.Controls.Add(subiconBox);
                        subiconBox.Size = new Size(imageSize / 2, imageSize / 2);
                        subiconBox.Location = new Point(imageSize / 2, imageSize / 2);
                        subiconBox.DoubleClick += new EventHandler(DoubleClick);

                        subiconBox.MouseDown += new MouseEventHandler(ClickDown);
                        subiconBox.MouseUp += new MouseEventHandler(ClickUp);
                        subiconBox.SizeMode = PictureBoxSizeMode.StretchImage;

                        subiconBox.Image = Image.FromFile(((Folder)file).GetIcon().path);

                        subiconBox.DragDrop += new DragEventHandler(DragDrop);
                        subiconBox.DragEnter += new DragEventHandler(DragEnter);
                        subiconBox.AllowDrop = true;
                        subiconBox.BringToFront();
                    }

                    iconBox.DragDrop += new DragEventHandler(DragDrop);
                    iconBox.DragEnter += new DragEventHandler(DragEnter);
                    iconBox.AllowDrop = true;
                    isFolder = true;
                }
            }

            if (IsDragged())
            {
                panel.BringToFront();
                panel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            }
            else
            {
                if (selected)
                {
                    panel.BackColor = System.Drawing.SystemColors.ActiveCaption;
                }
                else
                {
                    panel.BackColor = folderDisplay.container.BackColor;
                }
            }

            panel.Location = new System.Drawing.Point(GetX(), GetY());
            panel.Size = new System.Drawing.Size(GetW(), GetH());
        }

        public void Delete()
        {
            folderDisplay.RemoveItem(this);
            if (container != null)
                container.Controls.Remove(panel);
        }

        public override Constraints GetConstraints()
        {
            return Constraints.None;
        }

        public override void ClickDown(object sender, MouseEventArgs e)
        {
            //base.ClickDown(sender, e);

            var dob = new DataObject();
            dob.SetData(DataFormats.FileDrop, file.path);

            panel.DoDragDrop(dob, DragDropEffects.Copy);

            folderDisplay.dragDrop = true;

            if (e.Button == MouseButtons.Left)
            {
                if (clickTime > 0)
                {
                    DoubleClick(sender, e);
                }
                else
                {
                    clickTime = 50;
                }
            }

            if (e.Button != MouseButtons.Right)
            {
                if (Control.ModifierKeys != Keys.Shift && Control.ModifierKeys != Keys.Control)
                    folderDisplay.UnselectedAll();

                selected = !selected;
            }
            else
            {
                selected = true;
            }

            if (selected && e.Button != MouseButtons.Right)
            {
                if (file.isImage())
                {
                    try
                    {
                        preview.Image = Image.FromFile(file.path);
                    }
                    catch { }
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                if (file is FolderFile)
                    fileMenu.Show(panel, e.Location);
                else
                    directoryMenu.Show(panel, e.Location);
            }
        }

        public override void ClickUp(object sender, MouseEventArgs e)
        {
            //base.ClickUp(sender, e);
            folderDisplay.dragDrop = false;
        }

        public void DoubleClick(object sender, EventArgs e)
        {
            if (file is FolderFile)
            {
                ((FolderFile)file).Open();
            }
            else
            {
                folderDisplay.LoadFolder(new Folder(file.path));
            }
        }

        private void DragDrop(object sender, DragEventArgs e)
        {
            object tmp = (e.Data.GetData(DataFormats.FileDrop));
            string[] paths;
            if (tmp is string)
            {
                paths = new string[] { (string)tmp };
            }
            else
            {
                paths = (string[])tmp;
            }
            int amount = 0;
            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    new FolderFile(path).MoveTo(file.path);
                    amount++;
                }
                if (Directory.Exists(path) && path != file.path)
                {
                    new Folder(path).MoveTo(file.path);
                    amount++;
                }
            }
            if (amount > 0)
            {
                folderDisplay.Reload();
            }
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void LoadDrive()
        {
            var drive = ((DriveFolder)file);

            ProgressBar bar = new ProgressBar();
            bar.Size = new Size(128, 16);
            bar.Value = drive.GetPercent();
            bar.Maximum = 100;
            bar.Location = new Point(64, 32);
            panel.Controls.Add(bar);

            var label2 = new Label();
            panel.Controls.Add(label2);
            label2.Location = new Point(64, 2);
            label2.Size = new Size(128, 32);
            label2.AutoSize = false;
            label2.ForeColor = Color.White;
            label2.TextAlign = ContentAlignment.TopLeft;
            label2.AutoEllipsis = true;
            label2.Text = drive.GetName() + "\n" + drive.GetFreeSpace() + " free/" + drive.GetSize();

            label2.MouseDown += new MouseEventHandler(ClickDown);
            label2.MouseUp += new MouseEventHandler(ClickUp);

            bar.MouseDown += new MouseEventHandler(ClickDown);
            bar.MouseUp += new MouseEventHandler(ClickUp);
        }
    }
    public class FolderFile : FolderItem
    {
        public FolderFile(string file) : base(file)
        {

        }

        public override DateTime GetCreationDate()
        {
            return File.GetCreationTime(path);
        }

        public override DateTime GetModificationDate()
        {
            return File.GetLastWriteTime(path);
        }

        public override DateTime GetLateAccessDate()
        {
            return File.GetLastAccessTime(path);
        }

        public override FolderItem GetParent()
        {
            return new Folder(Directory.GetParent(path).FullName);
        }

        public void Open()
        {
            if (FileFilter.IsMusic(path))
            {
                //new MediaPlayer(path).Show();
            }
            else
            {
                try
                {
                    System.Diagnostics.Process.Start(path);
                }
                catch { }
            }
        }

        public string ExpandNamed(string end)
        {
            return GetParent().path + "\\" + name + end;
        }

        public string GetRealName()
        {
            return name.Substring(0, name.LastIndexOf("."));
        }

        public string GetExtension()
        {
            int i = name.LastIndexOf(".");
            if (i > -1)
                return name.Substring(i, name.Length - i);
            else
                return "";
        }

        public override void MoveTo(string folder)
        {
            if (Directory.Exists(folder))
            {
                if (!File.Exists(folder + "/" + name))
                    File.Move(path, folder + "/" + name);
            }
        }

        public override void CopyTo(string folder)
        {
            if (Directory.Exists(folder))
            {
                if (!File.Exists(folder + "/" + name))
                    File.Copy(path, folder + "/" + name);
            }
        }
    }
    public class DriveFolder : Folder
    {
        private DriveInfo info;

        public DriveFolder(string folder, DriveInfo info) : base(folder)
        {
            this.info = info;
        }

        public int GetPercent()
        {
            return (int)((1 - ((info.TotalFreeSpace * 1d) / (info.TotalSize * 1d))) * 100);
        }

        public string GetSize()
        {
            return (info.TotalSize / 1000_000_000).ToString() + " GB";
        }

        public string GetFreeSpace()
        {
            return (info.TotalFreeSpace / 1000_000_000).ToString() + " GB";
        }

        public string GetName()
        {
            return info.VolumeLabel;
        }
    }
    public class FileFilter
    {
        public static bool IsImage(string path)
        {
            var s = path.ToLower();

            return s.EndsWith(".png") || s.EndsWith(".jpeg") || s.EndsWith(".jpg") || s.EndsWith(".bmp") ||
                s.EndsWith(".gif") || s.EndsWith(".icon") || s.EndsWith(".ico");
        }

        public static bool IsMusic(string path)
        {
            var s = path.ToLower();

            return s.EndsWith(".wav") || s.EndsWith(".mp3") || s.EndsWith(".ogg");
        }

        public static bool IsPDF(string path)
        {
            var s = path.ToLower();

            return s.EndsWith(".pdf");
        }

        public static bool IsWord(string path)
        {
            var s = path.ToLower();

            return s.EndsWith(".docx") || s.EndsWith(".docm") || s.EndsWith(".doc") || s.EndsWith(".dotx") ||
                s.EndsWith(".dotm") || s.EndsWith(".rtf") || s.EndsWith(".odt");
        }

        public static bool IsExcel(string path)
        {
            var s = path.ToLower();

            return s.EndsWith(".xlsx") || s.EndsWith(".xlsm") || s.EndsWith(".xlsb") || s.EndsWith(".xls") ||
                s.EndsWith(".csv") || s.EndsWith(".prn") || s.EndsWith(".ods");
        }

        public static bool IsPowerPoint(string path)
        {
            var s = path.ToLower();

            return s.EndsWith(".pptx") || s.EndsWith(".pptm") || s.EndsWith(".ppt") || s.EndsWith(".pps") || s.EndsWith(".odp");
        }

        private List<Condition> conditions = new List<Condition>();

        public bool Contains(string path)
        {
            foreach (Condition condition in conditions)
            {
                if (!condition(path))
                {
                    return false;
                }
            }
            return true;
        }

        public void AddExtension(string extension)
        {
            conditions.Add((f) => f.ToLower().EndsWith(extension));
        }
        public void AddContain(string contains)
        {
            conditions.Add((f) => f.ToLower().Contains(contains));
        }
        public void AddSizeBigger(long value)
        {
            conditions.Add((f) => new FileInfo(f).Length > value);
        }
        public void AddSizeSmaller(long value)
        {
            conditions.Add((f) => new FileInfo(f).Length < value);
        }
        public void AddCreattionDateBigger(DateTime time)
        {
            conditions.Add((f) => new FileInfo(f).CreationTime > time);
        }
        public void AddCreationDateSmaller(DateTime time)
        {
            conditions.Add((f) => new FileInfo(f).CreationTime < time);
        }
        public void AddModificationDateBigger(DateTime time)
        {
            conditions.Add((f) => new FileInfo(f).LastWriteTime > time);
        }
        public void AddModificationDateSmaller(DateTime time)
        {
            conditions.Add((f) => new FileInfo(f).LastWriteTime < time);
        }
        public void AddFileStart(string arg)
        {
            conditions.Add((f) => new FolderFile(f).GetRealName().StartsWith(arg));
        }
        public void AddFileEnd(string arg)
        {
            conditions.Add((f) => new FolderFile(f).GetRealName().EndsWith(arg));
        }

        public delegate bool Condition(string path);
    }
}
