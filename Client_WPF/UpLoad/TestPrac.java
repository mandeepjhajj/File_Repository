

public class TestPrac {

	static int myStatic=10;
		
	
		public static void main(String[] args) {
		
			int myInt;
			myInt=100;
			
			System.out.println("Integer is :"+myInt);
			
			String name="Mandeep Singh Jhajj";
			String address= new String("Syracuse");

			String concat = name+ address;
			System.out.println("Name is : "+name);
			System.out.println("Address is : "+address);
			System.out.println("name and address: "+name+ " "+address);
			System.out.println("Concatenation is :"+ concat);

			System.out.println("Static variable is: "+ myStatic);


		}

}
