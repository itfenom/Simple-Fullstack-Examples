--Creating Java class:
CREATE OR REPLACE AND RESOLVE JAVA SOURCE NAMED MyJavaUtilityClass
/*CREATE OR REPLACE AND RESOLVE JAVA SOURCE NAMED MyJavaUtilityClass*/
AS 
import java.sql.*;  
import java.io.*;  
import java.util.List;  
import java.util.ArrayList;  
import oracle.sql.*;

public class MyJavaUtilityClass
{
  public static String GetHelloWorldMessage()
  {
    String retVal = "";
    try
    {
        retVal = "Hello world! From: Java class method 'GetHelloWorldMessage()'."; 
    }
    catch(Exception ex)
    {
      System.err.println(ex.getMessage());
    }
    return retVal;
  }
  
  public static boolean IsNumeric(String inputVal) 
  {
    try 
    {
      //int i = Integer.parseInt(inputVal);
	  double d = Double.parseDouble(inputVal);
    } 
    catch (Exception ex) 
    {
        System.err.println(ex.getMessage());
        return false;
    }
    return true;
  }

  public static String FormatName(String firstName, String lastName) 
  {
    String retVal = "";
    try 
    {
      retVal =  lastName + ", " + firstName;
    } 
    catch (Exception ex) 
    {
      retVal = ex.getMessage();
    }
    return retVal;
  }
   
  public static void ThrowExceptionMethod(int inputId, String inputValue) throws SQLException
  {
    throw new SQLException("Intentionally throwing an exception.");
  }
  
}/*end class MyJavaUtilityClass*/
---------
--Defining Java function inside a package:

FUNCTION GET_JAVA_MESSAGE RETURN VARCHAR2;

FUNCTION IS_NUMERIC(IN_INPUT_VAL VARCHAR2) RETURN BOOLEAN;

FUNCTION GET_FULL_NAME(IN_FIRST_NAME VARCHAR2, IN_LAST_NAME VARCHAR2) RETURN VARCHAR2;

PROCEDURE EXCEPTION_HANDLING(IN_ID NUMBER, IN_VALUE VARCHAR2);

 FUNCTION GET_JAVA_MESSAGE
 RETURN VARCHAR2
 AS LANGUAGE JAVA NAME 'MyJavaUtilityClass.GetHelloWorldMessage() return java.lang.String';
  
 FUNCTION IS_NUMERIC(IN_INPUT_VAL VARCHAR2)
 RETURN BOOLEAN
 AS LANGUAGE JAVA NAME 'MyJavaUtilityClass.IsNumeric(java.lang.String) return java.lang.Boolean';
 
 FUNCTION GET_FULL_NAME(IN_FIRST_NAME VARCHAR2, IN_LAST_NAME VARCHAR2) 
 RETURN VARCHAR2
 AS LANGUAGE JAVA NAME 'MyJavaUtilityClass.FormatName(java.lang.String, java.lang.String) return java.lang.String';
 
  PROCEDURE EXCEPTION_HANDLING(IN_ID NUMBER, IN_VALUE VARCHAR2) 
 AS LANGUAGE JAVA NAME 'MyJavaUtilityClass.ThrowExceptionMethod(int, java.lang.String)';
--- calling from anonymous block:
SET SERVEROUTPUT ON;
DECLARE
       V_EXCEPTION_MSG VARCHAR2(4000) := NULL;
       V_IS_NUMERIC BOOLEAN := NULL;
       V_VALUE VARCHAR2(1000) := '145.35';
       V_HELLO_WORLD_MSG VARCHAR2(200) := NULL;
       V_FORMATTED_NAME VARCHAR2(1000) := NULL;
       
BEGIN
       V_IS_NUMERIC := TEST_PKG.IS_NUMERIC(V_VALUE);
       V_HELLO_WORLD_MSG := TEST_PKG.GET_JAVA_MESSAGE();
       V_FORMATTED_NAME := TEST_PKG.GET_FULL_NAME('Kashif', 'Mubarak');
       TEST_PKG.EXCEPTION_HANDLING(1234, 'blaBla');

       DBMS_OUTPUT.PUT_LINE(V_HELLO_WORLD_MSG);
       
       DBMS_OUTPUT.PUT_LINE(V_FORMATTED_NAME);
       
       DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
       
       IF(V_IS_NUMERIC = TRUE)THEN
          DBMS_OUTPUT.PUT_LINE(V_VALUE || ' is numeric.'); 
       ELSE
          DBMS_OUTPUT.PUT_LINE(V_VALUE || ' is NOT numeric.');          
       END IF;
EXCEPTION
	 WHEN OTHERS THEN
      V_EXCEPTION_MSG := 'An error was encountered - ' || SQLCODE || ' -ERROR- ' || SQLERRM || ' -STACKTRACE- ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE;
      DBMS_OUTPUT.PUT_LINE(V_EXCEPTION_MSG);
END;
-----------------------------------------------------------------------------------------------------------------------------------------
CREATE TABLE Customers 
(
    CustNo NUMBER(3) NOT NULL,
    CustName VARCHAR2(30) NOT NULL,
    Street VARCHAR2(20) NOT NULL,
    City VARCHAR2(20) NOT NULL,
    State CHAR(2) NOT NULL,
    Zip VARCHAR2(10) NOT NULL,
    Phone VARCHAR2(12),
    PRIMARY KEY (CustNo)
);
--------------------------------------------------
CREATE TABLE Orders 
(
    PONo NUMBER(5),
    Custno NUMBER(3) REFERENCES Customers,
    OrderDate DATE,
    ShipDate DATE,
    ToStreet VARCHAR2(20),
    ToCity VARCHAR2(20),
    ToState CHAR(2),
    ToZip VARCHAR2(10),
    PRIMARY KEY (PONo)
);
--------------------------------------------------
CREATE TABLE StockItems 
(
    StockNo NUMBER(4) PRIMARY KEY,
    Description VARCHAR2(20),
    Price NUMBER(6,2)
);
--------------------------------------------------
CREATE TABLE LineItems 
(
    LineNo NUMBER(2),
    PONo NUMBER(5) REFERENCES Orders,
    StockNo NUMBER(4) REFERENCES StockItems,
    Quantity NUMBER(2),
    Discount NUMBER(4,2),
    PRIMARY KEY (LineNo, PONo)
);
--------------------------------------------------
CREATE OR REPLACE AND RESOLVE JAVA SOURCE NAMED POManager
/*CREATE OR REPLACE AND RESOLVE JAVA SOURCE NAMED POManager*/
AS 
import java.sql.*;
import java.io.*;
import oracle.jdbc.*;

public class POManager
{
public static void addCustomer (int custNo, String custName, String street,
   String city, String state, String zipCode, String phoneNo) throws SQLException
  {
    String sql = "INSERT INTO Customers VALUES (?,?,?,?,?,?,?)";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, custNo);
      pstmt.setString(2, custName);
      pstmt.setString(3, street);
      pstmt.setString(4, city);
      pstmt.setString(5, state);
      pstmt.setString(6, zipCode);
      pstmt.setString(7, phoneNo);
      pstmt.executeUpdate();
      pstmt.close();
    }
    catch (SQLException e) 
    {
      System.err.println(e.getMessage());
    }
  }

  public static void addStockItem (int stockNo, String description, float price)
                                                               throws SQLException
  {
    String sql = "INSERT INTO StockItems VALUES (?,?,?)";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, stockNo);
      pstmt.setString(2, description);
      pstmt.setFloat(3, price);
      pstmt.executeUpdate();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  public static void enterOrder (int orderNo, int custNo, String orderDate,
   String shipDate, String toStreet, String toCity, String toState,
    String toZipCode) throws SQLException 
  {
    String sql = "INSERT INTO Orders VALUES (?,?,?,?,?,?,?,?)";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, orderNo);
      pstmt.setInt(2, custNo);
      pstmt.setString(3, orderDate);
      pstmt.setString(4, shipDate);
      pstmt.setString(5, toStreet);
      pstmt.setString(6, toCity);
      pstmt.setString(7, toState);
      pstmt.setString(8, toZipCode);
      pstmt.executeUpdate();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  public static void addLineItem (int lineNo, int orderNo, int stockNo,
   int quantity, float discount) throws SQLException
  {
    String sql = "INSERT INTO LineItems VALUES (?,?,?,?,?)";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, lineNo);
      pstmt.setInt(2, orderNo);
      pstmt.setInt(3, stockNo);
      pstmt.setInt(4, quantity);
      pstmt.setFloat(5, discount);
      pstmt.executeUpdate();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  public static void totalOrders () throws SQLException 
  {
    String sql = "SELECT O.PONo, ROUND(SUM(S.Price * L.Quantity)) AS TOTAL " +
     "FROM Orders O, LineItems L, StockItems S " +
     "WHERE O.PONo = L.PONo AND L.StockNo = S.StockNo " +
     "GROUP BY O.PONo";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      ResultSet rset = pstmt.executeQuery();
      printResults(rset);
      rset.close();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  static void printResults (ResultSet rset) throws SQLException
  {
    String buffer = "";
    try
    {
      ResultSetMetaData meta = rset.getMetaData();
      int cols = meta.getColumnCount(), rows = 0;
      for (int i = 1; i <= cols; i++)
      {
        int size = meta.getPrecision(i);
        String label = meta.getColumnLabel(i);
        if (label.length() > size)
          size = label.length();
        while (label.length() < size)
          label += " ";
        buffer = buffer + label + " ";
      }
      buffer = buffer + "\n";
      while (rset.next())
      {
        rows++;
        for (int i = 1; i <= cols; i++)
        {
          int size = meta.getPrecision(i);
          String label = meta.getColumnLabel(i);
          String value = rset.getString(i);
          if (label.length() > size) 
            size = label.length();
          while (value.length() < size)
            value += " ";
          buffer = buffer + value + " ";
        }
        buffer = buffer + "\n";
      }
      if (rows == 0)
        buffer = "No data found!\n";
      System.out.println(buffer);
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  public static void checkStockItem (int stockNo) throws SQLException
  {
    String sql = "SELECT O.PONo, O.CustNo, L.StockNo, " + 
     "L.LineNo, L.Quantity, L.Discount " +
     "FROM Orders O, LineItems L " +
     "WHERE O.PONo = L.PONo AND L.StockNo = ?";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, stockNo);
      ResultSet rset = pstmt.executeQuery();
      printResults(rset);
      rset.close();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  public static void changeQuantity (int newQty, int orderNo, int stockNo)
                                                               throws SQLException
  {
    String sql = "UPDATE LineItems SET Quantity = ? " +
     "WHERE PONo = ? AND StockNo = ?";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, newQty);
      pstmt.setInt(2, orderNo);
      pstmt.setInt(3, stockNo);
      pstmt.executeUpdate();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }

  public static void deleteOrder (int orderNo) throws SQLException
  {
    String sql = "DELETE FROM LineItems WHERE PONo = ?";
    try
    {
      Connection conn = DriverManager.getConnection("jdbc:default:connection:");
      PreparedStatement pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, orderNo);
      pstmt.executeUpdate();
      sql = "DELETE FROM Orders WHERE PONo = ?";
      pstmt = conn.prepareStatement(sql);
      pstmt.setInt(1, orderNo);
      pstmt.executeUpdate();
      pstmt.close();
    }
    catch (SQLException e)
    {
      System.err.println(e.getMessage());
    }
  }
}/*end class POManager*/
----------------------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE po_mgr 
AS
    PROCEDURE add_customer (cust_no NUMBER, cust_name VARCHAR2,
    street VARCHAR2, city VARCHAR2, state CHAR, zip_code VARCHAR2,
    phone_no VARCHAR2);
    PROCEDURE add_stock_item (stock_no NUMBER, description VARCHAR2,
    price NUMBER);
    PROCEDURE enter_order (order_no NUMBER, cust_no NUMBER,
    order_date VARCHAR2, ship_date VARCHAR2, to_street VARCHAR2,
    to_city VARCHAR2, to_state CHAR, to_zip_code VARCHAR2);
    PROCEDURE add_line_item (line_no NUMBER, order_no NUMBER,
    stock_no NUMBER, quantity NUMBER, discount NUMBER);
    PROCEDURE total_orders;
    PROCEDURE check_stock_item (stock_no NUMBER);
    PROCEDURE change_quantity (new_qty NUMBER, order_no NUMBER,
    stock_no NUMBER);
    PROCEDURE delete_order (order_no NUMBER);
END po_mgr;

/

CREATE OR REPLACE PACKAGE BODY po_mgr 
AS
    PROCEDURE add_customer (cust_no NUMBER, cust_name VARCHAR2,
    street VARCHAR2, city VARCHAR2, state CHAR, zip_code VARCHAR2,
    phone_no VARCHAR2) AS LANGUAGE JAVA
    NAME 'POManager.addCustomer(int, java.lang.String,
    java.lang.String, java.lang.String, java.lang.String,
    java.lang.String, java.lang.String)';
    
    PROCEDURE add_stock_item (stock_no NUMBER, description VARCHAR2,
    price NUMBER) AS LANGUAGE JAVA
    NAME 'POManager.addStockItem(int, java.lang.String, float)';
    
    PROCEDURE enter_order (order_no NUMBER, cust_no NUMBER,
    order_date VARCHAR2, ship_date VARCHAR2, to_street VARCHAR2,
    to_city VARCHAR2, to_state CHAR, to_zip_code VARCHAR2)
    AS LANGUAGE JAVA
    NAME 'POManager.enterOrder(int, int, java.lang.String,
    java.lang.String, java.lang.String, java.lang.String,
    java.lang.String, java.lang.String)';
    
    PROCEDURE add_line_item (line_no NUMBER, order_no NUMBER,
    stock_no NUMBER, quantity NUMBER, discount NUMBER)
    AS LANGUAGE JAVA
    NAME 'POManager.addLineItem(int, int, int, int, float)';
    
    PROCEDURE total_orders
    AS LANGUAGE JAVA
    NAME 'POManager.totalOrders()';
    
    PROCEDURE check_stock_item (stock_no NUMBER)
    AS LANGUAGE JAVA
    NAME 'POManager.checkStockItem(int)';
    
    PROCEDURE change_quantity (new_qty NUMBER, order_no NUMBER,
    stock_no NUMBER) AS LANGUAGE JAVA
    NAME 'POManager.changeQuantity(int, int, int)';
    
    PROCEDURE delete_order (order_no NUMBER)
    AS LANGUAGE JAVA
    NAME 'POManager.deleteOrder(int)';
END po_mgr;

/
--------------------------------------------------------------------------------------------------------------------------------
BEGIN
  po_mgr.add_stock_item(2010, 'camshaft', 245.00);
  po_mgr.add_stock_item(2011, 'connecting rod', 122.50);
  po_mgr.add_stock_item(2012, 'crankshaft', 388.25);
  po_mgr.add_stock_item(2013, 'cylinder head', 201.75);
  po_mgr.add_stock_item(2014, 'cylinder sleeve', 73.50);
  po_mgr.add_stock_item(2015, 'engine bearning', 43.85);
  po_mgr.add_stock_item(2016, 'flywheel', 155.00);
  po_mgr.add_stock_item(2017, 'freeze plug', 17.95);
  po_mgr.add_stock_item(2018, 'head gasket', 36.75);
  po_mgr.add_stock_item(2019, 'lifter', 96.25);
  po_mgr.add_stock_item(2020, 'oil pump', 207.95);
  po_mgr.add_stock_item(2021, 'piston', 137.75);
  po_mgr.add_stock_item(2022, 'piston ring', 21.35);
  po_mgr.add_stock_item(2023, 'pushrod', 110.00);
  po_mgr.add_stock_item(2024, 'rocker arm', 186.50);
  po_mgr.add_stock_item(2025, 'valve', 68.50);
  po_mgr.add_stock_item(2026, 'valve spring', 13.25);
  po_mgr.add_stock_item(2027, 'water pump', 144.50);
  COMMIT;
END;
-----------------------------------------------------------
BEGIN
  po_mgr.add_customer(101, 'A-1 Automotive', '4490 Stevens Blvd',
    'San Jose', 'CA', '95129', '408-555-1212');
  po_mgr.add_customer(102, 'AutoQuest', '2032 America Ave',
    'Hayward', 'CA', '94545', '510-555-1212');
  po_mgr.add_customer(103, 'Bell Auto Supply', '305 Cheyenne Ave',
    'Richardson', 'TX', '75080', '972-555-1212');
  po_mgr.add_customer(104, 'CarTech Auto Parts', '910 LBJ Freeway',
    'Dallas', 'TX', '75234', '214-555-1212');
  COMMIT;
END;
-----------------------------------------------------------
BEGIN
  po_mgr.enter_order(30501, 103, '14-SEP-1998', '21-SEP-1998',
    '305 Cheyenne Ave', 'Richardson', 'TX', '75080');
  po_mgr.add_line_item(01, 30501, 2011, 5, 0.02);
  po_mgr.add_line_item(02, 30501, 2018, 25, 0.10);
  po_mgr.add_line_item(03, 30501, 2026, 10, 0.05);

  po_mgr.enter_order(30502, 102, '15-SEP-1998', '22-SEP-1998',
    '2032 America Ave', 'Hayward', 'CA', '94545');
  po_mgr.add_line_item(01, 30502, 2013, 1, 0.00);
  po_mgr.add_line_item(02, 30502, 2014, 1, 0.00);

  po_mgr.enter_order(30503, 104, '15-SEP-1998', '23-SEP-1998',
    '910 LBJ Freeway', 'Dallas', 'TX', '75234');
  po_mgr.add_line_item(01, 30503, 2020, 5, 0.02);
  po_mgr.add_line_item(02, 30503, 2027, 5, 0.02);
  po_mgr.add_line_item(03, 30503, 2021, 15, 0.05);
  po_mgr.add_line_item(04, 30503, 2022, 15, 0.05);

  po_mgr.enter_order(30504, 101, '16-SEP-1998', '23-SEP-1998',
    '4490 Stevens Blvd', 'San Jose', 'CA', '95129');
  po_mgr.add_line_item(01, 30504, 2025, 20, 0.10);
  po_mgr.add_line_item(02, 30504, 2026, 20, 0.10);
  COMMIT;
END;
-----------------------------------------------------------
SELECT * FROM CUSTOMERS;
SELECT * FROM Orders;
SELECT * FROM StockItems;
SELECT * FROM LineItems;
-----------------------------------------------------------
SET SERVEROUTPUT ON;
CALL dbms_java.set_output(2000);
CALL po_mgr.total_orders();
----------------------------------------------------------------------------------------------------------------------------------------------------------
