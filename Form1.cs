using System;
using System.Drawing;
using System.Net;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using ImageMagick;

namespace PicReImage
{
    public partial class Form1 : Form
    {
        string destinationPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        bool isRus = System.Globalization.CultureInfo.CurrentCulture.EnglishName.ToString().Contains("Russian");
        Bitmap bitmap;
        MagickImage image;
        public Form1()
        {
            InitializeComponent();
            saveButton.Text = isRus ? "Сохранить" : "Save";
            updateButton.Text = isRus ? "Обновить" : "Update";
            changeDestinationButton.Text = isRus ? "Сменить" : "Change";
            openDestinationPathFolderButton.Text = isRus ? "Открыть" : "Open";
            infoLabel.Text = isRus ? "Нажмите \"Обновить\", чтобы получить новое изображение" : "Click \"Update\" to get a new image";


            RegistryKey reg = Registry.CurrentUser.OpenSubKey("Software\\<Dan1meR4ik=12/>\\Pic.Re-Image", true);
            if (reg != null)
                destinationPath = reg.GetValue("DestinationPath", destinationPath).ToString();
            else
            {
                reg = Registry.CurrentUser.CreateSubKey("Software\\<Dan1meR4ik=12/>\\Pic.Re-Image");
                reg.SetValue("DestinationPath", destinationPath);
            }
            if (!Directory.Exists(destinationPath))
            {
                MessageBox.Show(isRus ?
                    "Заданная ранее папка сохранения изображений была перемещена или удалена.\n" +
                    "Все изображения будут сохранены на Ваш рабочий стол." :

                    "The previously specified image saving folder has been moved or deleted.\n" +
                    "All images will be saved to your desktop.",
                    isRus ? "Внимание" : "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                destinationPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                reg.SetValue("DestinationPath", destinationPath);
            }
            destinationPathLabel.Text = (isRus ? "Путь назначения: " : "Destination folder: ") + destinationPath;
        }
        private async void updateButton_Click(object sender, EventArgs e)
        {
            var url = "https://pic.re/image";
            var dir = "C:\\Windows\\TEMP\\update.jpg";
            infoLabel.Text = isRus ? "Пожалуйста, подождите... Не закрывайте окно, даже если оно не отвечает" : "Please, wait... Don't close even if it not responding";

            WebClient client = new WebClient();
            try
            {
                byte[] data = client.DownloadData(url);
                MemoryStream ms = new MemoryStream(data);

                bitmap?.Dispose();
                image?.Dispose();

                image = new MagickImage(ms);
                await image.WriteAsync(new FileInfo(dir), MagickFormat.Jpg);
                bitmap = new Bitmap(dir);
                pictureBox.Image = bitmap;
                infoLabel.Text = (isRus ? "Получено изображение: " : "Image received: ") + image.Width + "x" + image.Height;
                this.Text = "pic.re/image ("  + image.Width + "x" + image.Height + ")";
                saveButton.Text = isRus ? "Сохранить" : "Save";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + (isRus ? "\n\nВероятно, это проблема соединения с ресурсом pic.re\n" : "\n\nThis is probably a problem connecting to pic.re\n") + ex.InnerException, ex.Message);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            bool exist = true;
            if (!Directory.Exists(destinationPath))
            {
                exist = false;
                destinationPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                UpdateReg();
                destinationPathLabel.Text = (isRus ? "Путь назначения: " : "Destination folder: ") + destinationPath;
            }
            string filePath = destinationPath + "\\image_"
                + (DateTime.Now.Year
                + (DateTime.Now.Month < 10 ? "0" : "") + DateTime.Now.Month
                + (DateTime.Now.Day < 10 ? "0" : "") + DateTime.Now.Day + "_"
                + (DateTime.Now.Hour < 10 ? "0" : "") + DateTime.Now.Hour
                + (DateTime.Now.Minute < 10 ? "0" : "") + DateTime.Now.Minute
                + (DateTime.Now.Second < 10 ? "0" : "") + DateTime.Now.Second).ToString() + ".jpg";
            if (image != null)
            {
                image.Write(new FileInfo(filePath));
                infoLabel.Text = "Saved as " + "image_"
                + (DateTime.Now.Year
                + (DateTime.Now.Month < 10 ? "0" : "") + DateTime.Now.Month
                + (DateTime.Now.Day < 10 ? "0" : "") + DateTime.Now.Day + "_"
                + (DateTime.Now.Hour < 10 ? "0" : "") + DateTime.Now.Hour
                + (DateTime.Now.Minute < 10 ? "0" : "") + DateTime.Now.Minute
                + (DateTime.Now.Second < 10 ? "0" : "") + DateTime.Now.Second).ToString() + ".jpg!";
                saveButton.Text = isRus ? "Сохранить*" : "Save*";
            }
            else
            {
                MessageBox.Show(isRus ? "Изображение не было получено!" : "Image was not received!");
            }
            if (!exist)
            {
                MessageBox.Show(isRus ? "Заданная ранее папка сохранения была перемещена или удалена.\n" +
                    "Изображение сохранено на Ваш рабочий стол." :
                    "The previously specified image saving folder has been moved or deleted.\n" +
                    "This image has been saved to your desktop.",
                    isRus ? "Ошибка" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void changeDestinationButton_Click(object sender, EventArgs e)
        {
            var dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                destinationPath = folderBrowserDialog1.SelectedPath;
                destinationPathLabel.Text = (isRus ? "Путь назначения: " : "Destination folder: ") + destinationPath;
                UpdateReg();
            }
        }

        private void openDestinationPathFolderButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", destinationPath);
        }

        void UpdateReg()
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey("Software\\<Dan1meR4ik=12/>\\Pic.Re-Image", true);
            if (reg == null)
                reg = Registry.CurrentUser.CreateSubKey("Software\\<Dan1meR4ik=12/>\\Pic.Re-Image");
            reg.SetValue("DestinationPath", destinationPath);
        }
    }
}