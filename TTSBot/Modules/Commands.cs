using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Audio;
using System.Diagnostics;

namespace TTSBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        IAudioClient audioClient;

        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel."); return; }

            /* Create audio client */
            audioClient = await channel.ConnectAsync();
        }

        [Command("tts")]
        public async Task TTS([Remainder] string message)
        {
            message = $@"""{message}""";
            CreategTTS(message);
        }

        private void CreategTTS(string message)
        {
            string gTTSPath = @"C:\Users\Tom\Desktop\DiscordTTS\gTTS.py";
            Process proc = new Process();
            proc.StartInfo.FileName = "python";
            proc.StartInfo.WorkingDirectory = @"C:\Users\Tom\Desktop";
            proc.StartInfo.UseShellExecute = false;

            /* Create command with proper args and execute */
            proc.StartInfo.Arguments = string.Concat(gTTSPath, " ", message);

            proc.Start();
            proc.WaitForExit();
            proc.Close();
        }

        private Process CreateFFMPEG(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }

        private async Task SendAsync(IAudioClient client, string path)
        {
            using (var ffmpeg = CreateFFMPEG(path))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = client.CreateDirectPCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }
        }
    }
}
