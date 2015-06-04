
class Animal{
	void disp()
	{
		System.out.println("animal class");
	}
}
class Tiger extends Animal{
	void disp()
	{
		System.out.println("tiger class");
	}
	void hidden()
	{
		System.out.println("hidden");
	}
}


public class InstanceOf {

	public static void main(String[] args) {
		Animal a = new Animal();
		a.disp();
		
		Animal b = new Tiger();
		b.disp();
		
		if(a instanceof Animal)
		{
			System.out.println("animal object 1");
		}
		if(b instanceof Animal)
		{
			System.out.println("animal object 2");
		}
		if(a instanceof Tiger)
		{
			System.out.println("tiger object 3");
		}
		if(b instanceof Tiger)
		{
			System.out.println("tiger object 4");
		}

		Tiger c = new Tiger();
		if(c instanceof Animal)
		{
			System.out.println("Animal object 5");
		}
		if(c instanceof Tiger)
		{
			System.out.println("tiger object 6");
		}
		
		Tiger d = (Tiger)b;
		d.hidden();
	}

}
