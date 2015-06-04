
public class LinkedListimplementation {

	private class Node
	{
		Node next;
		Object data;
		Node(Object data, Node next)
		{
			this.data=data;
			this.next=next;
		}
		
		public Object getKey()
		{
			return data;
		}
		public void setkey(Object _data)
		{
			data=_data;
		}
		public Node getNext()
		{
			return next;
		}
		public void setNext(Node _next)
		{
			next=_next;
		}
		
	}
	
	private Node head;
	private Node mergeHead;
	private Node current=null, current1=null,current2,current3;
	private int count=0;
	public void addHead(Object item)
	{
		head = new Node(item,null);
		/*System.out.println(head.data);
		System.out.println(head.next);*/
	}
	public void addLast(Object item)
	{
			
		Node n = new Node(item,null);
		/*System.out.println(n.data);
		System.out.println(n.next);
		System.out.println(n);*/
		Node current = head;
		//It gave null pointer exception because head was null and object is getting 
		//created inside addhead Method. 
		//What I did wrong is that inside addHead { Node head = new Node(item,null)
		//Then after removing the Node from head..{ head = new Node(item,null)
		//It worked fine because now my head variable is only one. If I defined Node then 
		//I was actually having two variables by name head. 
		// One at class instance level and one inside the method addHead(). 
		//And in the addSecond() I was referring to instance head variable which is null. Since variable of addHead is initialized not the variable at instance level.
		while(current.getNext()!=null)
		{
			current= current.getNext();
		}
		current.setNext(n);
	}
	
	public void showContent()
	{
		System.out.println("content of linked list");
		Node current = head;
		while(current!=null)
		{
			System.out.println(current.getKey());
			current=current.getNext();
		}
	}
	public int searchKey(Object a)
	{
		Node current = head;
		int count=0;
		while(current!=null)
		{
			count=count+1;
			if(a==current.getKey())
			{
				return count;
			}
			current= current.getNext();
		}
		return 0;
	}
	public void addFirst(Object a)
	{
		Node current = head;
		System.out.println(head.getKey());
		Node n = new Node(a,null);
		n.setNext(current);
		head=n;
	}
	public void remove(Object a)
	{
		Node current = head;
		Node prev =null;
		while(current!=null)
		{
			if(current.getKey()==a)
			{
				prev.setNext(current.getNext());
			}
			prev=current;
			current= current.getNext();
			
		}
	}
	public static void main(String[] args) {
		
		LinkedListimplementation a = new LinkedListimplementation();
		a.addHead(1);
		a.addLast(2);
		a.addLast(3);
		a.addLast(44);
		a.addLast(55);
		a.addLast(80);
		a.addLast(90);
		a.showContent();
		
		int index= a.searchKey(9);
		if(index!=0)
			System.out.println("element present at location "+index+ " in list");
		else
			System.out.println("element is not present");
		
		/*a.addFirst(0);
		
		a.addLast(10);
		a.addFirst(-1);
		a.showContent();
		a.remove(8);
		a.showContent();*/
		
		LinkedListimplementation a1 = new LinkedListimplementation();
		a1.addHead(10);
		a1.addLast(20);
		a1.addLast(30);
		a1.addLast(40);
		a1.addLast(50);
		a1.addLast(60);
		a1.addLast(70);
		a1.showContent();
		
		LinkedListimplementation a2 = new LinkedListimplementation();
		
		Node m1 = a2.merge(a.head, a1.head);
		Node m2=m1;
		System.out.println("content of merged list");
		while(m1!=null)
		{
			System.out.println(m1.getKey());
			m1=m1.getNext();
		}
		
		Node rev = a2.reverse(m2);
		System.out.println("content of reversed list");
		while(rev!=null)
		{
			System.out.println(rev.getKey());
			rev=rev.getNext();
		}
	}
	
	//reverse the linked list
	
	public Node reverse(Node current)
	{
		Node prev=null;
		Node next = current.getNext();
		while(next!=null)
		{
			current.setNext(prev);
			prev=current;
			current=next;
			next=next.getNext();
		}
		current.setNext(prev);
		return current;
	}
	// Merge and sort the two sorted linked list using recursion
	
	public Node merge(Node l1, Node l2)
	{
		System.out.println("building merged list");
		
		
		if(l1==null && l2!=null)
		{
			current.setNext(l2);
			current = current.getNext();
			l2= l2.getNext();
			return merge(l1,l2);
		}
			
		if(l2==null && l1!=null)
		{
			current.setNext(l1);
			current = current.getNext();
			l1= l1.getNext();
			return merge(l1,l2);
		}
			
		if(l2==null && l1==null)
			return mergeHead;
		
		
		if((Integer)l1.getKey() <= (Integer)l2.getKey())
		{
			
			if(count==0)
			{
				mergeHead= l1;
				//mergeHead.setNext(null);
				current=mergeHead;
			}
			else
			{
				current.setNext(l1);
				current = current.getNext();
			}
			l1= l1.getNext();
			current2 = l1;
			while(current2!=null)
			{
				System.out.println(current2.getKey());
				current2=current2.getNext();
			}
			count++;
			return merge(l1,l2);
			
		}
		else
		{
			if(count==0)
			{
				mergeHead= l2;
				//mergeHead.setNext(null);
				current=mergeHead;
			}
			else
			{
				current.setNext(l2);
				current = current.getNext();
			}
			l2= l2.getNext();
			current3 = l2;
			while(current3!=null)
			{
				System.out.println(current3.getKey());
				current3=current3.getNext();
			}
			count++;
			return merge(l1,l2);
		}
		
		
	}

}
