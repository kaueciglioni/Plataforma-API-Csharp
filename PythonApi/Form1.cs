using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private string audioGuid = string.Empty;
        private string newAudioGuid = string.Empty;
        private string audioFilePath = string.Empty;


        public Form1()
        {
            InitializeComponent();
        }

        private async void CreateButton_Click(object sender, EventArgs e)
        {
            audioGuid = guidTextBox.Text;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    audioFilePath = openFileDialog.FileName;

                    string fileName = Path.GetFileName(audioFilePath);
                    string newFileName = $"{fileName}";

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.Add(new StringContent(audioGuid), "guid");

                        var fileStream = File.OpenRead(audioFilePath);
                        var streamContent = new StreamContent(fileStream);
                        streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                        {
                            Name = "audios",
                            FileName = newFileName
                        };
                        formData.Add(streamContent);

                        try
                        {
                            var response = await client.PostAsync("http://localhost:5000/Home/create", formData);
                            response.EnsureSuccessStatusCode();
                            var responseString = await response.Content.ReadAsStringAsync();
                            MessageBox.Show(responseString);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erro ao enviar o arquivo: " + ex.Message);
                        }
                        finally
                        {
                            fileStream.Close();
                        }
                    }
                }
            }
        }

        private async void DeleteButton_Click(object sender, EventArgs e)
        {
            audioGuid = guidTextBox.Text;
            try
            {
                var response = await client.DeleteAsync($"http://localhost:5000/Home/delete/{audioGuid}");
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                MessageBox.Show(responseString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao deletar o arquivo: " + ex.Message);
            }
        }

        private async void UpdateButton_Click(object sender, EventArgs e)
        {
            audioGuid = guidTextBox.Text;
            newAudioGuid = newGuidTextBox.Text;

            try
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new StringContent(newAudioGuid), "guid");

                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav|All files (*.*)|*.*";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            audioFilePath = openFileDialog.FileName;

                            string fileName = Path.GetFileName(audioFilePath);
                            string newFileName = $"{fileName}";

                            var fileStream = File.OpenRead(audioFilePath);
                            var streamContent = new StreamContent(fileStream);
                            streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                            {
                                Name = "audios",
                                FileName = newFileName
                            };
                            formData.Add(streamContent);

                            var response = await client.PutAsync($"http://localhost:5000/Home/update/{audioGuid}", formData);
                            response.EnsureSuccessStatusCode();

                            var responseString = await response.Content.ReadAsStringAsync();
                            MessageBox.Show(responseString);

                            fileStream.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar o arquivo: " + ex.Message);
            }
        }

        private async void GetButton_Click(object sender, EventArgs e)
        {
            string audioGuid = guidTextBox.Text;

            try
            {
                var response = await client.GetAsync($"http://localhost:5000/Home/{audioGuid}");
                response.EnsureSuccessStatusCode();

                var responseStream = await response.Content.ReadAsStreamAsync();

                string downloadsPath = "\\\\wsl.localhost\\Ubuntu\\home\\kaue\\voxProjects\\api-audios\\downloads";

                if (!Directory.Exists(downloadsPath))
                {
                    Directory.CreateDirectory(downloadsPath);
                }

                string fileName = $"{audioGuid}_downloaded_audio.mp3";
                string downloadFilePath = Path.Combine(downloadsPath, fileName);

                using (var fileStream = new FileStream(downloadFilePath, FileMode.Create, FileAccess.Write))
                {
                    await responseStream.CopyToAsync(fileStream);
                }

                MessageBox.Show($"Áudio baixado com sucesso em: {downloadFilePath}");

                System.Diagnostics.Process.Start("explorer.exe", downloadsPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao baixar o arquivo: {ex.Message}");
            }
        }
    }
}
