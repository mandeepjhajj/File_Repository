
interface ITest{
	void getInfo();
}

class Person implements ITest{
	private String name;
	
	public Person(String name) {
		super();
		this.name = name;
	}
	
	void showPerson(){
		System.out.println("This is person show");
	}

	public void getInfo() {
		System.out.println("name is :"+name);
	}
	
}

class Machine implements ITest{
	private int a=1;
	
	void showMachine(){
		System.out.println("Machine show");
	}

	public void getInfo() {
		System.out.println("Id is : "+a);
	}
	
}
public class Interface {

	public static void main(String[] args) {
	
		Person p1 = new Person("Mandeep");
		Machine m1 = new Machine();
		
		p1.showPerson();
		m1.showMachine();
		
		ITest i1= new Person("Bob");
		ITest i2 = new Machine();
		
		i1.getInfo();
		i2.getInfo();
		
		checkObject(p1);
	}
	public static void checkObject(ITest i3)
	{
		i3.getInfo(); //This will check at run time as which object is passed to the method, then it calls appropriate showinfo().
	}

}
