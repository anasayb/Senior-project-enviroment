<?php

    $con = mysqli_connect('localhost', 'root', '', 'trafficLightResults');

    // Check the connection
    if(mysqli_connect_errno())
    {
        echo("1"); //error code #1 = connection field
        exit();
    }

    if(count($_POST) == 0){

        // add times to the table
        $insertQuey = "SHOW TABLES;";
        $result = mysqli_query($con, $insertQuey) or die("4: Getting data Failed");

        if($result -> num_rows > 0){

            while($row = $result->fetch_assoc()){
                echo $row["Tables_in_trafficlightresults"] . " ";
            }

        }else{
            echo("0");
        }

    }else{

        // add times to the table
        $insertQuey = "SELECT * FROM `".$_POST["name"]."`;";
        $result = mysqli_query($con, $insertQuey) or die("4: Getting data Failed");

        if($result -> num_rows > 0){

            while($row = $result->fetch_assoc()){
                if($row["name"] == "AVG_Waiting_time"){
                    echo ($row["name"] . "_" . $row["waitingTime"]);
                }else{
                    echo ($row["name"] . "_" . $row["waitingTime"] . "_" . $row["Direction"] . "_" . $row["Streat"] . " ");
                }
            }

        }else{
            echo("0");
        }

    }
    
    $con->close();

?>