import java.util.*;

public class LinkedListStack {

	public static void main(String[] args) {
		LinkedList<Integer> l1 = new LinkedList<Integer>();
		
		l1.add(2);
		l1.add(3);
		l1.add(4);
		l1.add(5);
		l1.add(6);
		l1.add(7);
		l1.add(8);
		
		System.out.println("Linnked list content"+l1);
		l1.addFirst(0);
		l1.addLast(10);
		System.out.println("Linnked list content"+l1);
		l1.push(4);
		System.out.println("Linnked list content"+l1);
		l1.pop();
		System.out.println("Linnked list content"+l1);
		int a =l1.peekLast();
		System.out.println(a);
		System.out.println("Linnked list content"+l1);
	}

}
