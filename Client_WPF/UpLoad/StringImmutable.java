/*
 * fName.concat(lName);--- This creates a new object mandeepjhajj,
 * but the fname still refers to the mandeep. This is why string is immutable.
 * 
 * fName=fName.concat(lName); -----In this we explicitely assigned mandeepjhajj to fName. 
 * 
 * Note: Still mandeep object is not changed. It still remains in the string constant pool.
 * 
 * Suppose there are 5 reference variables,all referes to one object "mandeep".
 * If one reference variable changes the value of the object, 
 * it will be affected to all the reference variables. 
 * That is why string objects are immutable in java.
 * 
 * */
public class StringImmutable {
	
	
		public static void main(String[] args) {
		
			String fName="mandeep";
			String lName="jhajj";
						
			String name="mandeep";
			
			if(fName==name)
			{
				System.out.println("== operator success for name");
			}
			if(fName.equals(name))
			{
				System.out.println("equals operator success for name");
			}
			
			fName.concat(lName);
			System.out.println("variable fname has: "+fName);
			System.out.println("variable name has: "+name);
			
			fName=fName.concat(lName);
			System.out.println("variable fname has: "+fName);
			System.out.println("variable name has: "+name);
					
			
	}

}
