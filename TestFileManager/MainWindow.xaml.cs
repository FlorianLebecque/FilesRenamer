using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Security;
using NaturalSorting;
using System.Windows.Threading;

namespace TestFileManager {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        public class Dataobject {
            public Boolean Selected { get; set; }
            public string File_name { get; set; }
            public string New_File_name { get; set; }
            public Dataobject(Boolean c_,string t_,string m_) {
                Selected = c_;
                File_name = t_;
                New_File_name = m_;
            }
        }
 
        List<Dataobject> MyFiles = new List<Dataobject>();

        private void Button_Click(object sender, RoutedEventArgs e) {
            string Cur_folder = Tb_path.Text;

            string filter;

            if (Filter_tb.Text != "") {
                filter = Filter_tb.Text + ".*";
            }
            else {
                filter = "*.*";
            }

            Load_folder(Cur_folder, filter);
        }

        //folder loader
        private void Load_folder(string path, string filter) {
            treeView1.Items.Clear();
            checkBox1.IsChecked = false;
            DGV.ItemsSource = null;
            MyFiles.Clear();


            string[] AllFiles = Directory.GetFiles(path, filter, SearchOption.TopDirectoryOnly);

            Array.Sort(AllFiles, new AlphanumComparatorFast());

            TSL1.Content = "Listing folder";
            TSL2.Content = path;
            TSL3.Content = Convert.ToString(AllFiles.Count()) + " files";

            Tb_path.Text = path;

            TSpg1.Value = 0;
            TSpg1.Maximum = AllFiles.Count();

            //listing file
            foreach (var file in AllFiles) {
                //TSpg1.Dispatcher.Invoke(() => TSpg1.Value = TSpg1.Value + 1, DispatcherPriority.Background);
                TSpg1.Value++;
                FileInfo info = new FileInfo(file);
                MyFiles.Add(new Dataobject(false, info.Name, ""));
            }

            setDatagridviewdata();

            //listing folder
            DirectoryInfo Path_info = new DirectoryInfo(path);

            TreeViewItem root = new TreeViewItem();
            root.Header = Path_info.Name;

            string[] AllDirs = Directory.GetDirectories(path);
            TSpg1.Value = 0;
            TSpg1.Maximum = AllDirs.Count();
            foreach (var dir in AllDirs) {
                DirectoryInfo folder = new DirectoryInfo(dir);
                TreeViewItem itm = new TreeViewItem();
                itm.FontSize = 11;
                itm.Header = folder.Name;
                root.Items.Add(itm);
                //TSpg1.Dispatcher.Invoke(() => TSpg1.Value = TSpg1.Value + 1, DispatcherPriority.Background);
            }
            root.IsExpanded = true;
            treeView1.Items.Add(root);
            TSL1.Content = "Ready";
        }


        private void Button_Click_1(object sender, RoutedEventArgs e) {
            DirectoryInfo dir = new DirectoryInfo(Tb_path.Text + "/..");
            string Cur_folder = dir.FullName;

            string filter;

            if (Filter_tb.Text != "") {
                filter = Filter_tb.Text + ".*";
            }
            else {
                filter = "*.*";
            }

            Load_folder(Cur_folder, filter);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            TSL1.Content = "Selecting Folder";
            FBD.ShowDialog();

            string Cur_folder, filter;

            if (Filter_tb.Text != "") {
                filter = Filter_tb.Text;
            }
            else {
                filter = "*.*";
            }


            if (FBD.SelectedPath != "") {
                TSL1.Content = "Ready";
                Cur_folder = FBD.SelectedPath;
                Load_folder(Cur_folder, filter);
            }
            else {
                TSL2.Content = "No Folder selected";
                TSL3.Content = "0 files";
                TSL1.Content = "Canceled";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            string cur_dir, filter;
            filter = "*.*";

            cur_dir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            Load_folder(cur_dir, filter);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) {
            Filter_tb.Clear();
            TSL1.Content = "No Folder selected";
            TSL3.Content = "Ready";
            TSL2.Content = "No Folder selected";
            TSL3.Content = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            TSL3.Content = "No Folder selected";
            TSL3.Content = "0 files";
            Tb_path.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            TSpg1.Value = 0;
            Load_folder(Tb_path.Text, ".");
        }

        private void clear_DGV() {

            for (int i = 0; i < MyFiles.Count; i++) {
                MyFiles[i].New_File_name = "";
            }

            setDatagridviewdata();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e) {
            System.Windows.MessageBox.Show(" * (asterisk)Zero or more characters in that position." + Environment.NewLine + " ? (question mark) Zero or one character in that position.");
        }

        private void setDatagridviewdata() {
            DGV.ItemsSource = null;
            DGV.ItemsSource = MyFiles;

            DGV.Columns[0].CanUserSort = false;
            DGV.Columns[1].CanUserSort = false;
            DGV.Columns[2].CanUserSort = false;

            DGV.Columns[1].Width = new DataGridLength(50, DataGridLengthUnitType.Star);
            DGV.Columns[2].Width = new DataGridLength(50, DataGridLengthUnitType.Star);

            DGV.Columns[0].Header = "";
            DGV.Columns[1].IsReadOnly = true;
        }

        private void Filter_tb_TextChanged(object sender, TextChangedEventArgs e) {
            if (TSL2.Content.ToString() != "No Folder selected") {
                string Cur_folder = TSL2.Content.ToString();

                string filter;

                if (Filter_tb.Text != "") {
                    filter = Filter_tb.Text + ".*";
                }
                else {
                    filter = "*.*";
                }

                Load_folder(Cur_folder, filter);
            }
        }

        private void checkBox1_Click(object sender, RoutedEventArgs e) {
            if (checkBox1.IsChecked==true) {

                for (int i = 0; i < MyFiles.Count; i++) {
                    MyFiles[i].Selected = true;
                }

                setDatagridviewdata();

            }
            else {

                for (int i = 0; i < MyFiles.Count; i++) {
                    MyFiles[i].Selected = false;
                }

                setDatagridviewdata();
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if ((treeView1.SelectedItem != treeView1.Items[0]) && (treeView1.SelectedItem != null)) {
                TreeViewItem itm = (TreeViewItem)treeView1.SelectedItem;
                string Cur_folder = Tb_path.Text + "\\" + itm.Header;

                string filter;

                if (Filter_tb.Text != "") {
                    filter = Filter_tb.Text + ".*";
                }
                else {
                    filter = "*.*";
                }

                Load_folder(Cur_folder, filter);
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e) {
            if (tb_rep_input.Text != "") {
                clear_DGV();

                for (int i = 0; i < MyFiles.Count; i++) {
                    if (MyFiles[i].Selected) {
                        string temp = MyFiles[i].File_name;
                        temp = temp.Replace(tb_rep_input.Text, tb_rep_output.Text);
                        MyFiles[i].New_File_name = temp;
                    }
                }
                setDatagridviewdata();

            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e) {
            if (tb_base.Text != "") {

                clear_DGV();
                int offset = Convert.ToInt32(Nud1.Text);

                for (int i = 0; i < MyFiles.Count; i++) {
                    if (MyFiles[i].Selected) {
                        FileInfo f = new FileInfo(MyFiles[i].File_name);
                        string temp = f.Name;
                        string ext = f.Extension;

                        temp = tb_base.Text + "_" + offset.ToString() + ext;

                        MyFiles[i].New_File_name = temp;
                        offset++;

                    }
                }
                setDatagridviewdata();
            }
        }

        private void nud1up_Click(object sender, RoutedEventArgs e) {
            int value = Convert.ToInt32(Nud1.Text);
            value++;
            Nud1.Text = value.ToString();
        }

        private void nud1down_Click(object sender, RoutedEventArgs e) {
            int value = Convert.ToInt32(Nud1.Text);
            value = (value>0)? value-1:0;
            Nud1.Text = value.ToString();
        }

        private void Nud1_TextChanged(object sender, TextChangedEventArgs e) {
            int i = 0;
            foreach(char chr in Nud1.Text) {
                if (((int)chr <48)||((int)chr >57)){
                    Nud1.Text = Nud1.Text.Remove(i);
                }
                i++;
            }

            if(Nud1.Text == "") {
                Nud1.Text = "0";
            }
 
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            int i = 0;
            foreach (char chr in Nud2.Text) {
                if (((int)chr < 48) || ((int)chr > 57)) {
                    Nud2.Text = Nud2.Text.Remove(i);
                }
                i++;
            }

            if (Nud2.Text == "") {
                Nud2.Text = "0";
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e) {
            int value = Convert.ToInt32(Nud2.Text);
            value++;
            Nud2.Text = value.ToString();
        }

        private void Button_Click_8(object sender, RoutedEventArgs e) {
            int value = Convert.ToInt32(Nud2.Text);
            value = (value > 0) ? value - 1 : 0;
            Nud2.Text = value.ToString();
        }

        private void Button_Click_9(object sender, RoutedEventArgs e) {
            switch (comboBox2.SelectedIndex) {
                case 0: //left
                    clear_DGV();
                    for (int i = 0; i < MyFiles.Count; i++) {
                        if (MyFiles[i].Selected) {
                            int hm = Convert.ToInt32(Nud2.Text);

                            if (hm >= (MyFiles[i].File_name.Length)) {
                                hm = MyFiles[i].File_name.Length - 1;
                            }
                            string temp = MyFiles[i].File_name.Remove(0, hm);

                            MyFiles[i].New_File_name = temp;
                        }
                    }
                    setDatagridviewdata();
                    break;
                case 1://right
                    clear_DGV();
                    for (int i = 0; i < MyFiles.Count; i++) {
                        if (MyFiles[i].Selected) {
                            int hm = Convert.ToInt32(Nud2.Text);// how many
                            if (hm >= (MyFiles[i].File_name.Length)) {
                                hm = MyFiles[i].File_name.Length - 1;
                            }
                            string temp = MyFiles[i].File_name.Remove(MyFiles[i].File_name.Length - hm, hm);

                            MyFiles[i].New_File_name = temp;
                        }
                    }
                    setDatagridviewdata();
                    break;
            }
        }

        private void Button_Click_10(object sender, RoutedEventArgs e) {
            switch (comboBox3.SelectedIndex) {
                case 0: //left
                    clear_DGV();
                    for (int i = 0; i < MyFiles.Count; i++) {
                        if (MyFiles[i].Selected) {

                            string temp = tb_add.Text + MyFiles[i].File_name;

                            MyFiles[i].New_File_name = temp;
                        }
                    }
                    setDatagridviewdata();
                    break;
                case 1://right
                    clear_DGV();
                    for (int i = 0; i < MyFiles.Count; i++) {
                        if (MyFiles[i].Selected) {

                            string temp = MyFiles[i].File_name + tb_add.Text;

                            MyFiles[i].New_File_name = temp;
                        }
                    }
                    setDatagridviewdata();
                    break;
            }
        }

        private int Count_Checked() {

            int co = 0;
            for (int i = 0; i < MyFiles.Count; i++) {
                if (MyFiles[i].Selected) {
                    co += 1;
                }
            }
            return co;
        }

        private void Button_Click_11(object sender, RoutedEventArgs e) {
            TSL1.Content = "Renaming File";
            TSpg1.Value = 0;
            TSpg1.Maximum = Count_Checked();

            for (int i = 0; i < MyFiles.Count; i++) {
                if ((MyFiles[i].Selected)&&(MyFiles[i].New_File_name!="")) {
                    FileInfo file = new FileInfo(TSL2.Content + "/" + MyFiles[i].New_File_name);

                    //if file already exist, add "_ind"
                    int ind = 0;
                    string temp, temp2;
                    if (file.Extension != "") {
                        temp2 = file.Name.Replace(file.Extension, "");
                    }
                    else {
                        temp2 = file.Name;
                    }
                    temp = temp2;
                    while (file.Exists) {

                        temp = temp2 + "_" + ind.ToString();
                        file = new FileInfo(TSL2.Content + "/" + temp + file.Extension);
                        ind += 1;

                    }

                    File.Move(TSL2.Content + "/" + MyFiles[i].File_name, file.FullName);

                    TSpg1.Dispatcher.Invoke(() => TSpg1.Value = TSpg1.Value+1, DispatcherPriority.Background);
                }
            }
            Load_folder(TSL2.Content.ToString(), ".");
        }


        

    }
    

}
