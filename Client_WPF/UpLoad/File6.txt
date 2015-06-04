import java.io.File;
import java.util.ArrayList;
import java.util.List;

public class FileClass {

	public static void main(String[] args) {
		File f = null;
	      String[] strs = {"test1", "test2"};
	      String filenm[] = {"tmp1.txt","tmp2.txt","tmp3.txt","tmp4.txt"};
	      try{
	         // for each string in string array 
	         for(String s:strs )
	         {
	            // create new file
	            f= new File(s);
	           
	            // find the absolute path
	            String a = f.getAbsolutePath(); 
	            //create the directory
	            f.mkdirs();
	            for(String fl:filenm)
		         {
		        	 File fle= new File(a,fl);
		        	 //create the 4 text files in each directory
		        	 fle.createNewFile();
		         }
	            // prints absolute path
	            System.out.println(a);
	       
	            
	         } 
	         
	       //Search the file in the specified directory
	       File src = new File("M:\\WORKBECH\\HelloWorld");
	       String list[]= src.list();
	       
	       //list to hold the directories
	       ArrayList<String> dir= new ArrayList<String>();
	       
	       for(String s:list)
	       {
	    	   System.out.println(s);
	    	   
	       }
	       if(true)
	       {
	    	   for(String s:list)
	    	   {
	    		   File rDir = new File(s);
	    		   if(rDir.isDirectory())
	    		   {
	    			   dir.add(s);
	    		   }
	    	   }
	       }
	       System.out.println("Directories are : ");
	       for(String s :dir)
	       {
	    	   System.out.println(s);
	       }
	      }
	      catch(Exception e)
	      {
	         e.printStackTrace();
	      }
	}

}
