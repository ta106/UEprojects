import java.util.Queue;
import java.util.Vector;
/////////////////////////////////////////
/////////////////////////////////////////
//Authors: Turki , Mohammad
//CS470
//Dr. Hwang
//Process Management // RR algorithm
//process Class
/////////////////////////////////////////
public class process {
	
	private int PID;
	private int burstTime;
	private int EID;
	private boolean waiting = false;
	private Vector<process> sons = new Vector<process>();
	private writer w;
	private process parent;
	public boolean isDead = false;
	
	//Constructor for a real process
	//it needs to know its ID BurstTime and its parent
	//It also needs a "writer" to output results to
	public process(int PID, int burstTime, writer w, process parent){
		this.PID = PID;
		this.burstTime = burstTime;
		this.w = w;
		this.parent = parent;
	}
	//Constructor for original process that handles the rest of the processes
	//only needs to know that its process 0
	protected process() {
		this.PID = 0;
	}
	//Creates a string with information about the process
	// Its id and the remaining burst time
	// Also outputs what event its waiting on if any
	public String toString(){
		String info = ("PID " + PID + " " + burstTime);
		if(waiting) info += " " +EID;
		return info;
		
	}

	public int getPID() {
		return this.PID;
	}

	// decrements the burstTIme
	// terminates this process if it is done
	//then notifies its parent that it is done
	public void decrement() {
		
		burstTime--;
		if(burstTime == 0){
			
			terminate();
			this.parent.remove(this);
		}
	}


	//detaches a son from its parent
	private void remove(process son) {
		sons.remove(son);
	}
	
	//Creates a process and makes it this process's son
	// Adds that process to the ready queue
	public void addSon(int sonID, int sonBSize,writer w, Queue<process> readyQueue) {
		process n = new process(sonID,sonBSize,w,this);
		sons.addElement(n);
		readyQueue.add(n);
		w.append(n.toString() + " Placed on ReadyQueue");
		w.skipLine();
	}

	public int getEID() {
		return EID;
	}
	//Tells this process that it is waiting on a certin event
	public void waitOn(int Eid) {
		this.EID = Eid;
		waiting = true;
	}
	// terminates a process forcibly
	// only does so if it is that process to be terminated
	// or if it is a direct child of it
	public void terminate(int termeniated_PID) {
		if(this.PID == termeniated_PID)this.terminate();
		process terminatedSon = null;
		for(process each : sons){
			if(each.PID == termeniated_PID){
				each.terminate();
				terminatedSon = each;
			}
		}
		if(terminatedSon != null)sons.remove(terminatedSon);
	}
	// Terminates itself by declaring that it is dead to removed later
	// outputs that it is terminated
	// tells all its children to die
	private void terminate() {
		w.append(this.toString() + " terminated");
		w.skipLine();
		for(process each: sons)each.terminate();
		this.isDead = true;
	}
	// notifies this process that it is no longer waiting on any event
	public void release() {
		waiting = false;
		
	}
}
