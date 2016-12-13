package com.lukaszbalukin.orienteering.utils;

import java.io.File;
import java.io.IOException;
import java.io.StringWriter;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.Locale;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;
import org.xmlpull.v1.XmlSerializer;

import android.animation.IntEvaluator;
import android.util.Log;
import android.util.Xml;

import com.lukaszbalukin.orienteering.models.Checkpoint;
import com.lukaszbalukin.orienteering.models.OrienteeringRun;

public class Serializer
{
	public String SerializeRun(OrienteeringRun run)
			throws IllegalArgumentException, IllegalStateException, IOException
	{
		XmlSerializer xml = Xml.newSerializer();
		SimpleDateFormat sdf = new SimpleDateFormat("dd-MM-yyyy HH:mm:ss");

		StringWriter sw = new StringWriter();
		xml.setOutput(sw);
		xml.startDocument("UTF-8", true);

		xml.startTag("", "OrienteeringRun");

		xml.startTag("", "InProgress");
		xml.text(run.GetIsInProgress().toString());
		xml.endTag("", "InProgress");
		
		xml.startTag("", "TimeChanged");
		xml.text(run.GetIsCheating().toString());
		xml.endTag("", "TimeChanged");
		
		xml.startTag("", "Started");
		xml.text(sdf.format(run.GetDateStarted()));
		xml.endTag("", "Started");
		
		if(run.GetIsInProgress() == false)
		{
			xml.startTag("", "Ended");
			xml.text(sdf.format(run.GetDateEnded()));
			xml.endTag("", "Ended");
		}		

		xml.startTag("", "Header");
		xml.text(run.GetRunHeader());
		xml.endTag("", "Header");
		
		xml.startTag("", "RunnerID");
		xml.text(run.GetRunnerId() + "");
		xml.endTag("", "RunnerID");

		xml.startTag("", "Checkpoints");

		for (Checkpoint c : run.GetCheckpoints())
		{
			xml.startTag("", "Checkpoint");

			xml.startTag("", "Id");
			xml.text("" + c.GetId());
			xml.endTag("", "Id");

			xml.startTag("", "TimeVisited");
			xml.text(sdf.format(c.GetDateVisited()));
			xml.endTag("", "TimeVisited");
			
			xml.startTag("", "Order");
			xml.text("" + "" + c.GetOrder());
			xml.endTag("", "Order");

			xml.endTag("", "Checkpoint");
		}

		xml.endTag("", "Checkpoints");

		xml.endTag("", "OrienteeringRun");

		xml.endDocument();

		return sw.toString();

	}

	public OrienteeringRun DeserializeRun(String filePath)
			throws ParserConfigurationException, SAXException, IOException,
			ParseException
	{
		// Inicjalizuje czytnik XML
		File fXmlFile = new File(filePath);
		DocumentBuilderFactory dbFactory = DocumentBuilderFactory.newInstance();
		DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
		Document doc = dBuilder.parse(fXmlFile);
		doc.getDocumentElement().normalize();

		// Jeśli nieprawidłowy plik to nie robi nic dalej
		if (doc.getDocumentElement().getNodeName().equals("OrienteeringRun") == false)
		{
			return null;
		}

		Element root = doc.getDocumentElement();

		// Jesli prawidlowy, to deklaruje zmienne do zbudowania obiektu biegu
		Date dateStarted, dateEnded;
		String runHeader;
		ArrayList<Checkpoint> checkpoints = new ArrayList<Checkpoint>();
		Boolean isInProgress, isCheating;
		
		// Odczytuje stan biegu
		isInProgress = Boolean.parseBoolean(getTagValue("InProgress", root));
		
		// Odczytuje znacznik manipulacji czasem
		isCheating   = Boolean.parseBoolean(getTagValue("TimeChanged", root));
		
		// Odczytuje date
		SimpleDateFormat sdf = new SimpleDateFormat("dd-MM-yyyy HH:mm:ss");
		
		String tmp = getTagValue("Started", root);
		dateStarted = sdf.parse(tmp);
		
		if(isInProgress == false)
		{
			dateEnded = sdf.parse(getTagValue("Ended", root));
		}

		// Nagłówek
		runHeader = getTagValue("Header", root);
		
		// ID Biegacza
		int id = Integer.parseInt(getTagValue("RunnerID", root));

		// Checkpointy
		NodeList nList = doc.getElementsByTagName("Checkpoint");

		for (int temp = 0; temp < nList.getLength(); temp++)
		{

			Node nNode = nList.item(temp);
			if (nNode.getNodeType() == Node.ELEMENT_NODE)
			{

				Element eElement = (Element) nNode;

				int checkpointId = Integer
						.parseInt(getTagValue("Id", eElement));
				
				Date dateVisited = sdf.parse(getTagValue("TimeVisited",
						eElement));
				
				int order = Integer
						.parseInt(getTagValue("Order", eElement));

				Checkpoint c = new Checkpoint(checkpointId, dateVisited, order);
				checkpoints.add(c);

			}
		}

		OrienteeringRun run = new OrienteeringRun(dateStarted, runHeader, checkpoints); 
		run.SetIsInProgress(isInProgress);
		run.SetIsCheating(isCheating);
		run.SetRunnerId(id);
		return run;

	}

	private String getTagValue(String sTag, Element eElement)
	{
		NodeList nlList = eElement.getElementsByTagName(sTag).item(0)
				.getChildNodes();
		Node nValue = (Node) nlList.item(0);

		return nValue.getNodeValue();
	}
}
