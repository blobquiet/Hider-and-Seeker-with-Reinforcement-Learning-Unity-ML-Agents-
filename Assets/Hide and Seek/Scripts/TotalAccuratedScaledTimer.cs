using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TotalAccuratedScaledTimer : object {

	public float timer;

	private readonly float m_interval;
	private readonly Action m_callback;
		
	public TotalAccuratedScaledTimer(float interval, Action callback) {
		m_interval = interval;
		m_callback = callback;

		timer = Time.fixedTime + m_interval;
	}

	public void Step() {
		if(Time.fixedTime > timer) PerformCall();
	}
		
	public void PerformCall() {
		m_callback?.Invoke();
		timer = Time.fixedTime + m_interval;
	}
}
