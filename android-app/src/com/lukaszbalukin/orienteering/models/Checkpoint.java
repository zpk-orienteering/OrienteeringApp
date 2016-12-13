package com.lukaszbalukin.orienteering.models;

import java.util.Date;

public class Checkpoint
{
	int id;
	int checkpointUID;
	
	public int GetCheckpointUID()
	{
		return checkpointUID;
	}

	public void SetCheckpointUID(int checkpointUID)
	{
		this.checkpointUID = checkpointUID;
	}

	int order;
	Date dateVisited;	
	
	public Checkpoint(int id, Date dateVisited, int order)
	{		
		this.checkpointUID = id;
		this.order = order;
		this.dateVisited = dateVisited;
	}
	
	public int GetOrder()
	{
		return order;
	}

	public void SetOrder(int order)
	{
		this.order = order;
	}

	public int GetId()
	{
		return id;
	}
	
	public void SetId(int id)
	{
		this.id = id;
	}
	
	public Date GetDateVisited()
	{
		return dateVisited;
	}
}
