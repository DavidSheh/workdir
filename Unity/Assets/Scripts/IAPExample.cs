﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

public class IAPExample : MonoBehaviour {
	
	public List<string> productInfo = new List<string>();

	private static IAPExample _example;
	public static IAPExample Current{ get {  return _example;} } 

	void Awake()
	{
		_example = this;
	}

	
	[DllImport("__Internal")]
	private static extern void InitIAPManager();//初始化
	
	[DllImport("__Internal")]
	private static extern bool IsProductAvailable();//判断是否可以购买
	
	[DllImport("__Internal")]
	private static extern void RequstProductInfo(string s);//获取商品信息
	
	[DllImport("__Internal")]
	private static extern void BuyProduct(string s);//购买商品
	
	//测试从xcode接收到的字符串
	void IOSToU(string s){
		Debug.Log ("[MsgFrom ios]"+s);
	}
	
	//获取product列表
	void ShowProductList(string s){
		productInfo.Add (s);
		Debug.Log ("GetList:"+s);
	}


	public void BuyItem(string Item)
	{
		Debug.Log (Item);
		//Assets.Scripts.UI.UIControllor.Singleton.MaskEventObjectName = Item;
		BuyProduct (Item);

	}
	//获取商品回执
	void ProvideContent(string s)
	{
		//Assets.Scripts.UI.UIControllor.Singleton.ClearMaskEvent ();

		Debug.Log ("[MsgFrom ios]proivideContent : " + s);
		if (!string.IsNullOrEmpty (s)) {
			//Assets.Scripts.DataManagers.PaymentManager.Singleton.Add (s);

			Assets.Scripts.DataManagers.GamePlayerManager.Singleton.DoPaymentBuyKey (s);
		}


	   
	}
	
	
	// Use this for initialization
	void Start () {
		#if !UNITY_IOS || UNITY_EDITOR
		 //Destroy(this);
		#else
		 InitIAPManager();
		 Debug.Log("init iapmanager!");
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		//Assets.Scripts.DataManagers.PaymentManager.Singleton.Tick ();
	}

	public bool IsPaymentAvailabel { 
		get {
			return IsProductAvailable();
		}
	}
	
	void OnGUI(){
		return;

		if (!IsProductAvailable ())
			return;
		/*if(GUILayout.Button("Test 1",GUILayout.Width(200), GUILayout.Height(100)))
			TestMsg();
		
		GUILayout.Space (200);
		if(GUILayout.Button("Test 1",GUILayout.Width(200), GUILayout.Height(100)))
			TestSendString("This is a msg form unity3d\tt1\tt2\tt3\tt4");
		
		GUILayout.Space (200);
		if(GUILayout.Button("Test 1",GUILayout.Width(200), GUILayout.Height(100)))
			TestGetString();
		/********通信测试***********/

		var pay = Assets.Scripts. DataManagers.GamePlayerManager.Singleton.PaymentData;
		string list = string.Empty;
		bool f = true;
		foreach (var i in pay) {
			if (!f)
				list += "\t";
			f = false;
			list += i.BundleID;
		}
		
		if(Btn ("GetProducts")){
			if(!IsProductAvailable())
				throw new System.Exception("IAP not enabled");
			productInfo = new List<string>();


			RequstProductInfo(list);
		}
		
		GUILayout.Space(40);
		
		for(int i=0; i<productInfo.Count; i++){
			if(GUILayout.Button (productInfo[i],GUILayout.Height (100), GUILayout.MinWidth (200))){
				string[] cell = productInfo[i].Split('\t');
				Debug.Log ("[Buy]"+cell[cell.Length-1]);
				BuyProduct(cell[cell.Length-1]);
			}
		}
	}
	
	bool Btn(string msg){
		GUILayout.Space (100);
		return 	GUILayout.Button (msg,GUILayout.Width (100),GUILayout.Height(50));
	}
}
