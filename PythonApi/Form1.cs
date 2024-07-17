using PythonApi;
using System.Windows.Forms;
using System;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        private string m_audioGuid = string.Empty;
        private string m_newAudioGuid = string.Empty;
        private string m_audioFilePath = string.Empty;
        RequestApi Request = new RequestApi();


        public Form1()
        {
            InitializeComponent();
            Log.Logger.Debug("Form1", "Form1", "Compilação iniciada com sucesso");
        }

        private async void CreateButton_Click(object p_sender, EventArgs p_e)
        {
            m_audioGuid = guidTextBox.Text;

            using (OpenFileDialog l_openFileDialog = new OpenFileDialog())
            {
                l_openFileDialog.Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav|All files (*.*)|*.*";
                if (l_openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    m_audioFilePath = l_openFileDialog.FileName;

                    try
                    {
                        var l_responseString = await Request.CreateAudio(m_audioGuid, m_audioFilePath);
                        MessageBox.Show(l_responseString);
                    }
                    catch (Exception l_ex)
                    {
                        MessageBox.Show(l_ex.Message);
                    }
                }
            }
        }

        private async void DeleteButton_Click(object p_sender, EventArgs p_e)
        {
            m_audioGuid = guidTextBox.Text;
            try
            {
                var l_responseString = await Request.DeleteAudio(m_audioGuid);
                MessageBox.Show(l_responseString);
            }
            catch (Exception l_ex)
            {
                MessageBox.Show(l_ex.Message);
            }
        }

        private async void UpdateButton_Click(object p_sender, EventArgs p_e)
        {
            m_audioGuid = guidTextBox.Text;
            m_newAudioGuid = newGuidTextBox.Text;

            try
            {
                var l_responseString = await Request.UpdateAudio(m_audioGuid, m_newAudioGuid, m_audioFilePath);
                MessageBox.Show(l_responseString);
            }
            catch (Exception l_ex)
            {
                MessageBox.Show(l_ex.Message);
            }
        }

        private async void GetButton_Click(object p_sender, EventArgs p_e)
        {
            string l_audioGuid = guidTextBox.Text;

            try
            {
                var l_downloadFilePath = await Request.GetAudio(l_audioGuid);
                MessageBox.Show($"Áudio baixado com sucesso em: {l_downloadFilePath}");
                System.Diagnostics.Process.Start("explorer.exe", l_downloadFilePath);
            }
            catch (Exception l_ex)
            {
                MessageBox.Show(l_ex.Message);
            }
        }
    }
}
