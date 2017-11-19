﻿using HlwnOS.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace HlwnOS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileSystemController fsctrl;
        private UserInfo.Roles role;
        
        static MainWindow()
        {
        }

        public MainWindow(string path, UserInfo.Roles role)
        {
            InitializeComponent();
            try
            {
                fsctrl = new FileSystemController();
                fsctrl.openSpace(path);
                openDirectory(fsctrl.CurrDir);
            }
            catch
            {
                //TODO
            }

            this.role = role;
            //TODO 18.11: менять функционал для админа/пользователя
        }

        private void openDirectory(string path)
        {
            wrapPanel.Children.Clear();
            path = UsefulThings.clearExcessSeparators(path);

            try
            {
                byte[] dir = fsctrl.readFile(path);
                FileHeader fh;
                while (dir.Length > 0)
                {
                    fh = new FileHeader();
                    fh.fromByteArray(dir);
                    if (fh.Name.First() != UsefulThings.DELETED_MARK)
                    {
                        if (!fh.isHidden || fh.isHidden && showHiddenChb.IsChecked.Value)
                        {
                            FileView fv = new FileView(fh);
                            fv.MouseDoubleClick += new MouseButtonEventHandler(onFileViewDoubleClick);
                            wrapPanel.Children.Add(fv);
                        }
                    }
                    dir = dir.Skip(FileHeader.SIZE).ToArray();
                }
                fsctrl.CurrDir = path;
                addressEdit.Text = fsctrl.CurrDir;
                if (fsctrl.CurrDir == "/")
                    backImg.IsEnabled = false;
                else
                    backImg.IsEnabled = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка");
                openDirectory(fsctrl.CurrDir);
            }
        }

        private void onFileViewDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileView senderFV = (sender as FileView);
            if (senderFV.FileHeader.isDirectory)
            {
                openDirectory(fsctrl.CurrDir + "/" + UsefulThings.truncateZeros(senderFV.FileHeader.Name));
            }
            else
            {
                //TODO
            }
        }

        private void onBackLMBUp(object sender, MouseButtonEventArgs e)
        {
            string newPath, tmp;
            UsefulThings.detachLastFilename(fsctrl.CurrDir, out newPath, out tmp);
            openDirectory(newPath);
        }

        private void logout(object sender, RoutedEventArgs e)
        {
            fsctrl.closeSpace();
            Session.clear();
            AuthWindow aw = new AuthWindow();
            Close();
            aw.Show();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                openDirectory(addressEdit.Text);
                addressEdit.SelectionStart = addressEdit.Text.Length;
            }
        }
    }
}
