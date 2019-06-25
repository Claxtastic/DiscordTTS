if __name__=='__main__':
	import sys
	from gtts import gTTS

	tts = gTTS(text=sys.argv[1], lang='en')
	tts.save("msg.mp3")