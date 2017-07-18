#pragma strict
import System.Xml;
import System.IO;

var paths : Array;
function spriteToString(mySprite : Sprite){
	var bytes : byte[] = mySprite.texture.EncodeToPNG();
	return System.Convert.ToBase64String(bytes);
}

function stringToSprite(byte64String : String){
	var bytes : byte[] = System.Convert.FromBase64String(byte64String);
 	var tex : Texture2D = new Texture2D(100,100);
 	tex.LoadImage(bytes);
 	return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.0f));
}

function writeToXml(points : Array,description : String, image : String){
	var xmlDoc : XmlDocument;
	xmlDoc = new XmlDocument();
	var xmlDeclaration : XmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0","utf-8",null);
    var rootNode : XmlElement = xmlDoc.CreateElement("ImageDescription");
    xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement); 
    xmlDoc.AppendChild(rootNode);
    var parentNode : XmlElement;
	parentNode = xmlDoc.CreateElement("Image");
	xmlDoc.DocumentElement.AppendChild(parentNode);
	parentNode.InnerText = image;
	var answerNode : XmlElement = xmlDoc.CreateElement("Answer");
	answerNode.InnerText = description;
	parentNode.AppendChild(answerNode);
	var lineNode : XmlElement = xmlDoc.CreateElement("point");
	answerNode.AppendChild(lineNode);

		/*for(int i=0;i<points.Count;i++){
			
			var xNode : XmlElement = xmlDoc.CreateElement("X");
			pointNode.AppendChild(xNode);
			xNode.InnerText = points[i].x.ToString();
			var yNode : XmlElement = xmlDoc.CreateElement("Y");
			pointNode.AppendChild(yNode);
			yNode.InnerText = points[i].y.ToString();
		}*/
	xmlDoc.Save(Application.persistentDataPath+"/Test.qz");
	//xmlDoc.Save(Application.persistentDataPath+"/check.qz");
}

function Settings(){
if(File.Exists(Application.persistentDataPath+"/settings.xml")){
		readXML(Application.persistentDataPath+"/settings.xml", paths, "path");
		return paths[0];
	}else{
		var xmlDoc : XmlDocument = new XmlDocument();
        var xmlDeclaration : XmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0","utf-8",null);
        var rootNode : XmlElement = xmlDoc.CreateElement("Settings");
        xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement); 
        xmlDoc.AppendChild(rootNode);
        var parentNode : XmlElement = xmlDoc.CreateElement("path");
        xmlDoc.DocumentElement.PrependChild(parentNode);
        parentNode.InnerText = Application.persistentDataPath;
        xmlDoc.Save(Application.persistentDataPath+"/settings.xml");
        return Application.persistentDataPath;
	}
}
function readXML(filepath : String, result : Array, tagName : String){
    var xmlDoc : XmlDocument = new XmlDocument();
    if(File.Exists (filepath))
    { 	
    	var x : XmlNodeList;
        xmlDoc.Load( filepath );
        x = xmlDoc.GetElementsByTagName(tagName);
		for (var i=0;i<x.Count;i++)
  		{ 
  			result.push(x.Item(i).InnerText);
  		}
	}
}
