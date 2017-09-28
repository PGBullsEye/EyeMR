'''
(*)~----------------------------------------------------------------------------------
Author: Tim Cofala and Henrik Reichmann
This backend is based on the fake_backend created by Pupil Labs and was extended
to allow receiving video data from an Unity application via a network stream.
----------------------------------------------------------------------------------~(*)
'''

from plugin import Plugin
from pyglui import ui
from time import time, sleep
from threading import Thread, Lock

import video_capture

import cv2
import socket
import numpy as np


#logging
import logging
logger = logging.getLogger(__name__)

class Frame(object):
	"""
		A frame object contains an image and corresponding information. The design of this class
		is copied from the fake_backend by Pupil Labs.
	"""
	def __init__(self, timestamp, img, index, heightX, widthY, channel):
		self.timestamp = timestamp
		self._img = img
		self.bgr = img
		self.height, self.width, _ = heightX, widthY, channel
		self.mode = 0
		if channel == 1:
			self._gray = self._img
		else:
			self._gray = None
		self.index = index
		# indicate that the frame does not have a native yuv or jpeg buffer
		self.yuv_buffer = None
		self.jpeg_buffer = None

	@property
	def img(self):
		return self._img

	@property
	def gray(self):
		if self._gray is None:
			self._gray = cv2.cvtColor(self._img, cv2.COLOR_BGR2RGB)
		return self._gray

	@gray.setter
	def gray(self, value):
		raise Exception('Read only.') 

class Unity_Stream_Plugin_Source(video_capture.Base_Source):

	def __init__(self, g_pool):
		super().__init__(g_pool)
		self.frame_count = 0
		self.presentation_time = time()
		self.fps = 30
		self.lock = Lock()
		self.img_for_this_frame = None
		self.thread_running = True

		port = 8051
		
		# To connect to a different device running a unity app we need some connection stuff
		self.socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
		self.socket.bind  (("", port ))
		self.socket.settimeout(3)
		self.network_thread()

	def recent_events(self, events):
		
		self.lock.acquire()
		if self.img_for_this_frame is None: # We were not able to receive date through the network stream.
			self.lock.release()
			return
		
		# Get time infos and other stuff
		now = time()
		spent = now - self.presentation_time
		wait = max(0, 1. / self.fps - spent)
		sleep(wait)
		self.presentation_time = time()
		frame_count = self.frame_count
		self.frame_count += 1
		timestamp = self.g_pool.get_timestamp()
		frame = Frame(timestamp, self.img_for_this_frame, self.frame_count, self.height, self.width, self.mode)
		self.lock.release()
		cv2.putText(frame.img, "Unity Source Frame %s" % self.frame_count, (20, 20), cv2.FONT_HERSHEY_SIMPLEX, 0.5,
					(255, 100, 100))
		events['frame'] = frame
		
		# Most importend. Set the world views image for this frame
		self._recent_frame = frame

	def network_thread(self):
	
		def run():
			while True:
			
				self.lock.acquire()
				if not self.thread_running:
					self.lock.release()
					return
				
				self.lock.release()

				# Lets try to get some data
				try:
					data, adr = self.socket.recvfrom(64000)
				except socket.timeout:
					logger.warning("No connection could be established. ")
					self.lock.acquire()
					self.img_for_this_frame = None
					self.lock.release()
					continue
								
				# Recreate the frame
				npArray1d = np.fromstring(data, dtype=np.uint8)
				
				mode = npArray1d[0]
				npArray1d = np.delete(npArray1d, [0])

				width = 0
				height = 0

				if mode == 3:
					width = 195
					height = 109
					npArray3d = np.reshape(npArray1d, (height, width, 3))
					npArray3d = cv2.cvtColor(npArray3d, cv2.COLOR_BGR2RGB)
					npArray3d = cv2.flip(npArray3d, 0)
				else:
					width = 330
					height = 185
					npArray3d = np.reshape(npArray1d, (height, width, 1))
					npArray3d = cv2.flip(npArray3d, 0)
					# frameU = cv2.imdecode(frameU, cv2.IMREAD_COLOR)
				
				self.lock.acquire()
				self.height = height
				self.width = width
				self.mode = mode
				self.img_for_this_frame = npArray3d
				self.lock.release()
                              
		self.network_thread = Thread(target = run, args = ())
		self.network_thread.start()

	def cleanup(self):
		self.info_text = None
		self._img = None
		self.preferred_source = None

		# Close the thread
		self.lock.acquire()
		self.thread_running = False
		self.lock.release()
		self.network_thread.join()
		
		self.socket.shutdown(2)
		self.socket.close()
		super().cleanup()
		
	@property
	def name(self):
		return 'Unity stream source' 
		
	@property
	def frame_size(self):
		return (1280, 720)

	@property
	def frame_rate(self):
		return 30
		
		
class Unity_Stream_Plugin_Manager(video_capture.Base_Manager):

	gui_name = 'Unity stream'

	def __init__(self, g_pool):
		super().__init__(g_pool)
			
	def init_gui(self):
		self.create_menu()
	
	
	def create_menu(self):
		""" Edit the menu here: 
		 
		Info text
		 
		|Start/Stop Button|
		
		"""
		#Button functions
		def activate():
			
			settings = {}
			self.notify_all({'subject': 'recording.should_stop'})
			self.notify_all({'subject': 'start_plugin', 'target': self.g_pool.process, "name": "Unity_Stream_Plugin_Source", 'args': settings})
			
		#UI elements
		start_button = ui.Button('Start streaming', activate)
		discription_text = ui.Info_Text('A backend for streaming video input from Unity. ')
		
		# Add UI elements here
		ui_elements = [
						discription_text,
						start_button
		] 
		
		self.g_pool.capture_selector_menu.extend(ui_elements)
		
	
	def recent_events(self, events):
		"""
			Used to prevent the falsly thrown deprecated warning. 
		"""
		pass
			
	
