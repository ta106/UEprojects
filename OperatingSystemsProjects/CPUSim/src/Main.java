/////////////////////////////////////////
/////////////////////////////////////////
//Authors: Turki , Mohammad
//CS470
// Dr. Hwang
//Process Management // RR algorithm
//Main Class
/////////////////////////////////////////
import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.util.InputMismatchException;
import java.util.Scanner;
import java.util.Vector;


public class Main {
	
	private static Vector<String> commands = new Vector<>();
	private static  Scanner scanner;
	
	//Main Function
	//Reads File name and Quantum size
	//Reads the commands from the specified file
	// creates a "Scheduler" and Asks to execute the given commands
	//Creates a "writer" and asks it to output the result
	
	public static void main(String[] args) {
		//Receiving data from the user + reading commands from file
		scanner = new Scanner(System.in);
		System.out.println("Files must be in project dir to be read");
		System.out.println("Enter file name");
		String outputName = readCommands();
		System.out.println("Enter quantem time");
		int QuantemTime;
		
		while(true)
			try{
				QuantemTime = scanner.nextInt();
				break;
			}catch(InputMismatchException e){
				System.out.println(e.getMessage());
			}
		
		//executing commands and outputing results
		writer w = new writer(outputName,QuantemTime);
		scheduler scheduler = new scheduler(QuantemTime,w);
		
		for(int i=0;i<commands.size();i++){
			if(!scheduler.execute(commands.elementAt(i)))break;
			
		}
		
		w.write();
		scanner.close();

	}
	//Asks the user for file name
	//reads commands from that fille
	//returns the file name to main
	private static String readCommands() {
		String FileName;
		while(true){
			FileName = scanner.nextLine();
			BufferedReader br = null;
			FileReader fr = null;
			boolean read = false;
			try {

				fr = new FileReader(FileName);
				br = new BufferedReader(fr);

				String sCurrentLine;

				br = new BufferedReader(new FileReader(FileName));

				while ((sCurrentLine = br.readLine()) != null) {
					parseCommands(sCurrentLine);
				}
				read = true;
			} catch (IOException e) {
				System.out.println(e.getMessage());
				e.printStackTrace();
				System.out.println("enter filename");
			} finally {

				try {

					if (br != null)
						br.close();

					if (fr != null)
						fr.close();
					if(read)break;
				} catch (IOException ex) {

					ex.printStackTrace();

				}

			}
		}
		return FileName;
	}
	//Recieves a line of commands and splits them by commas
	private static void parseCommands(String commandLine) {
		String cmds[] = commandLine.split(",");
		for(int i=0;i<cmds.length;i++)
		commands.addElement(cmds[i]);
	}

}
