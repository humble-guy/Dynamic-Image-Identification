using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System;
using System.Collections.Generic;


public class ManageXML : MonoBehaviour {

	List<string> paths;
	string defaultPath;
	public ManageXML(){
		paths = new List<string> ();
		defaultPath = Settings ();
		paths = new List<string>();
	}

	public string spriteToString(Sprite mySprite){
		byte[] bytes = mySprite.texture.EncodeToPNG();
		return System.Convert.ToBase64String(bytes);
	}

	public Sprite stringToSprite(string byte64String ){
		byte[] bytes  = System.Convert.FromBase64String(byte64String);
		Texture2D tex  = new Texture2D(100,100);
		tex.LoadImage(bytes);
		return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.0f));
	}

	public void createInfoFile(Vector2[] points,string description,string regionName,string image,string fileName){
		XmlDocument xmlDoc = new XmlDocument ();
		XmlNode rootNode;
		if (!File.Exists (defaultPath + "/"+fileName+".qz")) {

			XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration ("1.0", "utf-8", null);
			 rootNode = xmlDoc.CreateElement ("ImageDescription");
			xmlDoc.InsertBefore (xmlDeclaration, xmlDoc.DocumentElement); 
			xmlDoc.AppendChild (rootNode);
			XmlElement imageNode = xmlDoc.CreateElement ("image");
			//Debug.Log (image);
			imageNode.InnerText = image;
			rootNode.AppendChild (imageNode);
		} else {
			xmlDoc.Load(defaultPath + "/"+fileName+".qz");
			rootNode = xmlDoc.GetElementsByTagName ("ImageDescription").Item(0);
			}
		//Debug.Log (image);
	    XmlElement regionNode;

		regionNode = xmlDoc.CreateElement(regionName);

		XmlElement descNode= xmlDoc.CreateElement("description");
		descNode.InnerText = description;
		regionNode.AppendChild(descNode);
		XmlElement pointNode= xmlDoc.CreateElement("point");

		for(int i=0;i<points.Length;i++){

			// Now we need to create each inventory item		
			XmlElement inventoryItem = xmlDoc.CreateElement("item");
			inventoryItem.SetAttribute("x", points[i].x.ToString());
			inventoryItem.SetAttribute("y", points[i].y.ToString());       
			pointNode.AppendChild(inventoryItem);
		}
		regionNode.AppendChild(pointNode);
		rootNode.AppendChild (regionNode);
		xmlDoc.Save(defaultPath + "/"+fileName+".qz");
	}

	public string Settings(){
		if(File.Exists(Application.persistentDataPath+"/settings.xml")){
			readXML(Application.persistentDataPath+"/settings.xml", paths, "path");
			return paths[0];
		}
		else{
			XmlDocument xmlDoc = new XmlDocument();
			XmlDeclaration  xmlDeclaration  = xmlDoc.CreateXmlDeclaration("1.0","utf-8",null);
			XmlElement rootNode = xmlDoc.CreateElement("Settings");
			xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement); 
			xmlDoc.AppendChild(rootNode);
			XmlElement parentNode = xmlDoc.CreateElement("path");
			xmlDoc.DocumentElement.PrependChild(parentNode);
			parentNode.InnerText = Application.persistentDataPath;
			xmlDoc.Save(Application.persistentDataPath+"/settings.xml");
			return Application.persistentDataPath;
		}
	}

	public void readXML(string filepath, List<string> result,string tagName){
		
	    XmlDocument xmlDoc= new XmlDocument();
		if(File.Exists (filepath))
		{ 	
			  XmlNodeList x;
			xmlDoc.Load( filepath );
			x = xmlDoc.GetElementsByTagName(tagName);
			Debug.Log (x);
			for (var i=0;i<x.Count;i++)
			{ 
				result.Add(x.Item(i).InnerText);
			}
		}
	}
	//this method adds new path to settings.xml from where the files will be read or written into
	public void editSettings(string path){
		if(Directory.Exists(path)){
			 XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(Application.persistentDataPath+"/settings.xml");
		XmlNodeList x;
		x = xmlDoc.GetElementsByTagName("path");
         XmlNode itemPath = x.Item(x.Count-1);
		itemPath.InnerText = path;
		xmlDoc.Save(Application.persistentDataPath+"/settings.xml");
		Application.LoadLevel("start");
	}


}
}