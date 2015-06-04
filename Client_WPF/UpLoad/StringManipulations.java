/*
 * When we are using objects and want to check if they are equal, 
 * the operator == will say if they are the same, if you want to 
 * check if they are logically equal, you should use the equals method on the object. 
 * */
public class StringManipulations {

	void compTo(){
		
		String s1 ="Sachin";
		String s2 ="Tendulkar";
		String s3 = "Ramesh";
		
		s1.compareTo(s3);
		
		int array[]= new int[5];
		for(int i=0;i<5;i++)
		{
			array[i]=i;
		}
		for(int i : array)
		{
			System.out.println(i);
		}
	}
	void substrng()
	{
		String name="mandeep";
		String surnm="jhajj";
		
		String comp1= name.substring(1,2);
		System.out.println(comp1);
		
		String comp2=surnm.substring(2,3);
		System.out.println(comp2);
		
		if(comp1==comp2)
		{
			System.out.println("== success in substring");
		}
		if(comp1.equals(comp2))
		{
			System.out.println("equals success in substring");
		}
	}
	
	public static void main(String[] args) {
		 
		/* Difference between the equals and == operator*/
		
			String first= new String("firstobject");
			String second=new String("firstobject");
			String third= "firstobject";
			String fourth=first;
			String fifth= "firstobject";
			
			String check1="mandeep";
			String check2= "mandeep";
			
			if(first==second)
			{
				System.out.println("== operator is successful");
			}
			if(first.equals(second))
			{
				System.out.println("equals operator is successful");
			}
			if(first==third)
			{
				System.out.println("== using third object");
			}
			if(first.equals(third))
			{
				System.out.println("equals in third object");
			}
			if(first==fourth)
			{
				System.out.println("== in fourth object");
			}
			if(first.equals(fourth))
			{
				System.out.println("equals in fourth object");
			}
			if(third==fifth)
			{
				System.out.println("== in fifth object");
			}
			if(third.equals(fifth))
			{
				System.out.println("equals in fifth object");
			}
			
			
			if(check1==check2)
			{
				System.out.println("== operator is successful for chk1 and ckh2");
			}
			if(check1.equals(check2))
			{
				System.out.println("equals operator is successful for chk1 and ckh2");
			}
			
			StringManipulations obj1= new StringManipulations();
			
			obj1.substrng();
	}

}
