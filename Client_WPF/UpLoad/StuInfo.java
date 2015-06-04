import java.awt.*;
import java.awt.event.*;
import java.applet.*;
import java.sql.*;
import javax.sql.*;
//import java.io.*;


class StuInfo extends Frame implements ActionListener,ItemListener  
{
 
 Label lsid,lname,ladd,lmail,lphone,lgender,lbranch,lex,lex1,lhobby;
 TextField tsid,tname,tmail,tphone;
 TextField tadr;
 Checkbox rbm,rbf,cri,hoc,swim,read;
 CheckboxGroup cbg;
 Button bsubmit,bedit,bdelete,brst;
 Choice branch;
 String sgender,scri,shoc,sswim,sread,sbranch,sname,smail,sphone,sadd,ggender;
  int id,getid;

 Connection con;
 Statement stmt;
 PreparedStatement pstmt;
 ResultSet rs;

 StuInfo()
 {
	
   	lsid = new Label("Student ID : ");
	lname= new Label("Name : ");
	ladd = new Label("Address : ");
	lmail = new Label("Email ID : ");
	lphone = new Label("Phone no. : ");
	lgender = new Label("Gender : ");
	lbranch = new Label("Branch : ");
	lex = new Label("CHOOSE ANY ONE FROM BELOW ");
	lhobby = new Label("Hobbies :");
	lex1 = new Label("SELECT FROM BELOW ");

	tsid = new TextField();
	tname = new TextField();
	tmail = new TextField();
	tphone = new TextField();
	tadr= new TextField();

	bsubmit =new Button("Submit");
	bedit = new Button("Edit");
	bdelete = new Button("Delete");
	brst = new Button("Reset");
	
	cri = new Checkbox("Cricket ");
	hoc = new Checkbox("Hockey");
	swim = new Checkbox("Swimming ");
	read = new Checkbox("Reading");

	cbg = new CheckboxGroup();
	rbm = new Checkbox("Male",cbg,false);
	rbf = new Checkbox("Female",cbg,false);

	branch = new Choice();
	branch.add("------Select-------");
	branch.add("Electronics");
	branch.add("Computers");
	branch.add("ec");
	branch.add("IT");
	branch.add("Mehanical");
	branch.add("Civil");
	branch.select("------Select-------");
	

	setLayout(new BorderLayout());
	Panel p = new Panel();
	p.setLayout(new GridLayout(13,2,10,10));
	p.add(lsid);
	p.add(tsid);
	p.add(lname);
	p.add(tname);
	p.add(ladd);
	p.add(tadr);
	p.add(lmail);
	p.add(tmail);
	p.add(lphone);
	p.add(tphone); 
	p.add(lgender);
	p.add(lex);
	p.add(rbm); 
	p.add(rbf);
	p.add(lbranch);
	p.add(branch);
	p.add(lhobby);
	p.add(lex1);
	p.add(cri);
	p.add(hoc);
	p.add(swim);
	p.add(read);
	p.add(bsubmit);
	p.add(bedit);
	p.add(bdelete);
	p.add(brst);
	add(p,BorderLayout.CENTER);

	bsubmit.addActionListener(this);
	bedit.addActionListener(this);
	bdelete.addActionListener(this);
	brst.addActionListener(this);

	rbm.addItemListener(this);
	rbf.addItemListener(this);
	branch.addItemListener(this);
	cri.addItemListener(this);
	hoc.addItemListener(this);
	swim.addItemListener(this);
	read.addItemListener(this);

try{
 System.out.println("kkkkkkkkkk");	
  Class.forName("sun.jdbc.odbc.JdbcOdbcDriver");
System.out.println("hhhhhh");
     con=DriverManager.getConnection("jdbc:odbc:info","scott","tiger");

 stmt=con.createStatement();
	  
	}catch(Exception e){System.out.println(""+e); }
}//constructor


public void itemStateChanged(ItemEvent ie)
{
 sgender=cbg.getSelectedCheckbox().getLabel();

 if(cri.getState())   scri="Cricket";  else scri=null ;
 if(hoc.getState())  shoc="Hockey"  ; else shoc=null ;
 if(swim.getState()) sswim="Swimming"  ; else sswim=null ;
 if(read.getState())   sread="Reading"; else sread=null ;

sbranch=branch.getSelectedItem();
}

public void actionPerformed(ActionEvent ae) 
{
System.out.println("inside actionperformed");
 Object ob = ae.getSource();
 
System.out.println("inside try");
 if(ob==bsubmit)
{
try{
                id= Integer.parseInt(tsid.getText().trim());
	System.out.println("after id"+id);
 	sname = tname.getText().trim();
	System.out.println(" name :"+sname);
 	smail =tmail.getText();
	sadd =tadr.getText();
 	sphone = tphone.getText().trim();
	System.out.println(""+sphone);
  	System.out.println("inside submit");
pstmt= con.prepareStatement("insert into student values(?,?,?,?,?,?,?,?,?,?,?)");

 pstmt.setInt(1,id);
System.out.println("after id");
 pstmt.setString(2,sname);
 pstmt.setString(3,sadd);
 pstmt.setString(4,smail);
System.out.println("after mail");
 pstmt.setString(5,sphone);
 pstmt.setString(6,sgender);
 pstmt.setString(7,sbranch);

                 if(scri==null)
  	    pstmt.setString(8,"null");
	 else
	    pstmt.setString(8,scri);


 	 if(shoc==null)
  	    pstmt.setString(9,"null");
	 else
	    pstmt.setString(9,shoc);


	 if(sswim==null)
  	    pstmt.setString(10,"null");
	 else
	    pstmt.setString(10,sswim);


	 if(sread==null)
  	    pstmt.setString(11,"null");
	 else
	    pstmt.setString(11,sread);

System.out.println("after setstring");
 int i = pstmt.executeUpdate();
System.out.println("after update");
 if(i == 1)
 System.out.println("id inserted");
rs.close();
stmt.close();
pstmt.close();
con.close();
}catch(Exception e){System.out.println(e);}
}
if(ob==bedit)
{
  try{
  getid=Integer.parseInt(tsid.getText().trim());	
  stmt=con.createStatement();
  rs=stmt.executeQuery("select * from student where id="+getid);
        if(rs==null)
            System.out.println("Record Not Found");
        else 
        {
         rs.next();
         System.out.println("Record Found");
         tsid.setText(rs.getInt(1)+ " ");
         tname.setText(rs.getString(2));
         tadr.setText(rs.getString(3));
         tmail.setText(rs.getString(4));
         tphone.setText(rs.getString(5));
         ggender=rs.getString(6);
         System.out.println(""+ggender);        
        }
rs.close();
stmt.close();
pstmt.close();
con.close();
  }catch(Exception e){ System.out.println(e);}
}
if(ob==bdelete)
{
   try { 
         pstmt = con.prepareStatement("delete from student where id=?");
         getid=Integer.parseInt(tsid.getText().trim());
         pstmt.setInt(1,getid);
	 int i=pstmt.executeUpdate();
         if(i==1)
            System.out.println("Your data deleted successfully Student code is  : "+getid);
         else
            System.out.println("sorry");
rs.close();
stmt.close();
pstmt.close();
con.close();
       }catch(Exception e){}
}
if(ob==brst)
{
       tsid.setText(null);
       tname.setText(null);
       tphone.setText(null);
       tmail.setText(null);
       tadr.setText(null);
       branch.select("----Select---");	
}
 
}

public static void main(String arg[])
{
 StuInfo f= new StuInfo();
 f.setSize(600,600);
 f.setVisible(true);
 }
}