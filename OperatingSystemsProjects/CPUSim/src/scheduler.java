import java.util.LinkedList;
import java.util.Queue;
import java.util.Vector;
/////////////////////////////////////////
/////////////////////////////////////////
//Authors: Turki , Mohammad
//CS470
//Dr. Hwang
//Process Management // RR algorithm
//Scheduler Class
/////////////////////////////////////////


public class scheduler {
	
	private Queue<process> readyQueue =  new LinkedList<process>();
	private Queue<process> waitQueue =  new LinkedList<process>();
	private process currentPid;
	private process initProcess = new originalProcess();
	private int Quantem;
	private char cmd;
	private int args[] = new int [2];
	private process doneWaiting;
	private writer w;
	private int init_quantem;
	//Receives initial quantum and a writer to output results to
	//sets the current running process to process 0
	public scheduler(int init_quantem,writer w){
		this.w = w;
		this.init_quantem = init_quantem;
		this.Quantem = init_quantem;
		this.currentPid = this.initProcess;
	}
	//Takes a command
	//prints the current state
	//parse the command than
	//gives it to the currently running "process" to execute
	public boolean execute(String command) {
		w.skipLine();
		doneWaiting = null;
		printState();
		w.append(command);
		w.skipLine();
		parseCommand(command);
		switch (cmd){
		
		case 'C':
			currentPid.addSon(args[0],args[1],w,readyQueue);
			break;
		case 'D':
			currentPid.terminate(args[0]);
			removeDeadProcesses();
			break;
		case 'I':
			break;
		case 'W':
			if(currentPid.getPID() != 0){
				decrement();
				w.append(currentPid.toString() + " Placed on WaitQueue");
				w.skipLine();
				eventWait();
			}
			return true;
		case 'E':
			if(!waitQueue.isEmpty()){
				for(process each : waitQueue){
					if(each.getEID() == args[0])doneWaiting = each;
				}
				
			}
			break;
		case 'X':
			w.append("Current simulation state:");
			w.skipLine();
			printState();
			return false;
			
		
		}
		decrement();
		scheduleProcesses();
		
		
		return true;
	}

	//Takes care of the command W
	//Tells the current process to wait on the specified event
	//Puts that process on wait queue
	// Polls the next process to work
	//resets the quantum
	private void eventWait() {
		currentPid.waitOn(args[0]);
		waitQueue.add(currentPid);
		if(!readyQueue.isEmpty())
			currentPid = readyQueue.poll();
		else
			currentPid = this.initProcess;
		Quantem = init_quantem;
	}
	//Takes care of scheduling process
	// checks if the current process is done or if its quantum time is done to run the next one on the queue
	//checks if there is a process that is done waiting to put it back to the readyqueue
	//checks if no real process is running to pull a real one from the ready queue if there is one there
	private void scheduleProcesses() {
		if(currentPid.isDead){
			removeDeadProcesses();
			if(readyQueue.isEmpty()){
				currentPid = this.initProcess;
			}else{
				currentPid = readyQueue.poll();
				
			}
			Quantem = init_quantem;
		}else
			if(Quantem == 0){
				readyQueue.add(currentPid);
				currentPid = readyQueue.poll();
				Quantem = init_quantem;
			}
		
		if(currentPid.getPID() == 0 && !readyQueue.isEmpty())currentPid = readyQueue.poll();
		
		if(doneWaiting != null){
			waitQueue.remove(doneWaiting);
			readyQueue.add(doneWaiting);
			doneWaiting.release();
			w.append(doneWaiting.toString() + " Placed on ReadyQueue\n");
		}
	}
	// loops through queues to remove terminated processes
	private void removeDeadProcesses() {
		
		Vector<process> deadProcesses = new Vector<>();
		
		for(process each: readyQueue){
			if(each.isDead)deadProcesses.addElement(each);
		}
		for(process each : deadProcesses)readyQueue.remove(each);
		
		deadProcesses.clear();
		
		for(process each: waitQueue)
			if(each.isDead)deadProcesses.addElement(each);
		
		for(process each: deadProcesses)waitQueue.remove(each);
		
	}
	
	private void decrement() {
		if(currentPid.getPID() != 0)Quantem--;
		currentPid.decrement();
	}


	// parses the command by spaces 
	private void parseCommand(String command) {
		String[] splitter = command.split(" ");
		cmd = splitter[0].charAt(0);
		if(splitter.length > 1)args[0] =Integer.parseInt( (splitter[1]));
		if(splitter.length > 2)args[1] = Integer.parseInt(splitter[2]);
	}
	//prints the current state
	// that is what is the current running process
	// what processes are in the ready and wait queues
	private void printState() {
		
		w.append(currentPid.toString());
		
		if(currentPid.getPID() != 0){
			w.append(" Runing with " + Quantem + " left");
			w.skipLine();
		}else{
			w.append(" Runing");
			w.skipLine();
		}
		w.append("ReadQueue: ");
		for(process each : readyQueue){
			w.append(each.toString() + " ");
		}
		w.skipLine();
		w.append("WaitQueue: ");
		for(process each : waitQueue){
			w.append(each.toString());
		}
		w.skipLine();
	}
	
}
