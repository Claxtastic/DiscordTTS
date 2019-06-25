from gtts import gTTS

def createTTS(username, message):
	tts = gTTS(text=username + '' + message, lang='en')
	tts.save("msg.mp3")