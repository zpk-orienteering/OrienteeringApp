package com.lukaszbalukin.orienteering.models;

import java.util.ArrayList;
import java.util.Date;

public class OrienteeringRun
{
	int id;
	int runnerId;
	private Boolean isInProgress;
	private Boolean isCheating;

	public Boolean GetIsCheating()
	{
		return isCheating;
	}

	public void SetIsCheating(Boolean isCheating)
	{
		this.isCheating = isCheating;
	}

	public Boolean GetIsInProgress()
	{
		return isInProgress;
	}

	public void SetIsInProgress(Boolean isInProgress)
	{
		this.isInProgress = isInProgress;
	}

	public int GetId()
	{
		return id;
	}

	public int GetRunnerId()
	{
		return runnerId;
	}

	public void SetRunnerId(int runnerId)
	{
		this.runnerId = runnerId;
	}

	public void SetId(int id)
	{
		this.id = id;
	}

	Date dateStarted;
	Date dateEnded;

	public Date GetDateEnded()
	{
		return dateEnded;
	}

	public void SetDateEnded(Date dateEnded)
	{
		this.dateEnded = dateEnded;
	}

	String runHeader;
	ArrayList<Checkpoint> checkpoints;

	public Date GetDateStarted()
	{
		return dateStarted;
	}

	public String GetRunHeader()
	{
		return runHeader;
	}

	public ArrayList<Checkpoint> GetCheckpoints()
	{
		return checkpoints;
	}

	public OrienteeringRun(Date dateStarted, String runHeader,
			ArrayList<Checkpoint> checkpoints)
	{
		this.dateStarted = dateStarted;
		this.runHeader = runHeader;
		this.checkpoints = checkpoints;
		this.runnerId = 0;
		this.isCheating = false;
		this.isInProgress = true;
	}

	public OrienteeringRun()
	{
		// TODO Auto-generated constructor stub
	}
}
