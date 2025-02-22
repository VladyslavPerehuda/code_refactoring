using ChatLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;

namespace ChatClient
{
    public partial class Form1 : Form, IClientCallback
    {
        private IChatService _proxy;
        private DuplexChannelFactory<IChatService> _factory;
        private string _nickname;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void ReceiveMessage(string fromUser, string message)
        {
            Invoke(new Action(() =>
            {
                txtChat.AppendText($"{fromUser}: {message}{Environment.NewLine}");
            }));
        }

        public void UpdateUserList(string[] users)
        {
            Invoke(new Action(() =>
            {
                lstUsers.Items.Clear();
                lstUsers.Items.AddRange(users);
            }));
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            _nickname = txtNickname.Text.Trim(); 

            if (string.IsNullOrEmpty(_nickname))
            {
                MessageBox.Show("Please enter a nickname.");
                return;
            }

            btnConnect.Enabled = false;

            try
            {
                await Task.Run(() =>
                {
                    var instanceContext = new InstanceContext(this);

                    _factory = new DuplexChannelFactory<IChatService>(
                        instanceContext,
                        new NetTcpBinding(),
                        new EndpointAddress("net.tcp://localhost:8080/ChatService"));

                    _proxy = _factory.CreateChannel();

                    if (!_proxy.Connect(_nickname))
                    {
                        throw new Exception("Цей нікнейм вже використовується.");
                    }
                });

                MessageBox.Show("Підключено до чату!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка підключення: {ex.Message}");
            }
            finally
            {
                btnConnect.Enabled = true;
            }
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            SendMessage(message);
            txtMessage.Clear();
        }

        private void SendMessage(string message)
        {
            if (lstUsers.SelectedItem != null)
            {
                string toUser = lstUsers.SelectedItem.ToString();
                _proxy.SendPrivateMessage(_nickname, toUser, message);
            }
            else
            {
                _proxy.SendMessage(_nickname, message);
            }
        }


        private async Task SendMessageAsync(string message)
        {
            await Task.Run(() =>
            {
                _proxy.SendMessage(_nickname, message);
            });
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_proxy != null)
            {
                try
                {
                    _proxy.Disconnect(_nickname);
                    if (_factory != null && _factory.State == CommunicationState.Opened)
                    {
                        _factory.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while disconnecting: {ex.Message}");
                    _factory.Abort();
                }
            }
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
