using Core.Models;
using Microsoft.Win32;
using Services;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Client.UI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NetworkClient network = new NetworkClient();

        private Folder? selectedFolder;

        public MainWindow()
        {
            InitializeComponent();
            RefreshAll();
        }

        private void RefreshAll()
        {
            LoadFolders();
            LoadFiles(null);
        }

        private void LoadFolders()
        {
            var folders = network.GetFolders(Session.CurrentUser!.Id);

            FoldersTree.Items.Clear();

            foreach (var folder in folders.Where(f => f.ParentFolderId == null))
            {
                FoldersTree.Items.Add(CreateTreeItem(folder, folders));
            }
        }

        private void LoadFiles(Guid? folderId)
        {
            var files = network.GetFiles(Session.CurrentUser!.Id, folderId);

            FilesList.ItemsSource = null;   // важно для обновления UI
            FilesList.ItemsSource = files;
        }

        private TreeViewItem CreateTreeItem(Folder folder, List<Folder> all)
        {
            var item = new TreeViewItem
            {
                Header = folder.Name,
                Tag = folder
            };

            foreach (var child in all.Where(f => f.ParentFolderId == folder.Id))
                item.Items.Add(CreateTreeItem(child, all));

            return item;
        }

        private void FoldersTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (FoldersTree.SelectedItem is TreeViewItem item)
            {
                selectedFolder = item.Tag as Folder;
                LoadFiles(selectedFolder?.Id);
            }
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                network.UploadFile(
                    dialog.FileName,
                    Session.CurrentUser!.Username,
                    selectedFolder?.Id
                );

                LoadFiles(selectedFolder?.Id);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (FilesList.SelectedItem is not FileEntity file)
                return;

            network.DeleteFile(file.Id);
            LoadFiles(selectedFolder?.Id);
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            if (FilesList.SelectedItem is not FileEntity file)
                return;

            network.RenameFile(file.Id, "renamed_" + file.Name);
            LoadFiles(selectedFolder?.Id);
        }
    }
}
