<?php

    $con = mysqli_connect('localhost', 'root', '', 'trafficLightResults(Two)');

    // Check the connection
    if(mysqli_connect_errno())
    {
        echo("1"); //error code #1 = connection field
        exit();
    }

    // Create the table
    $insertQuey = "SHOW TABLES;";
    $result = mysqli_query($con, $insertQuey) or die("4: Getting data Failed");
    while($row = $result->fetch_assoc()){
                $url = 'http://localhost/sqlconnect/DeleteExistingData.php';
                $data = array('table' => 'information');

                // use key 'http' even if you send the request to https://...
                $options = array(
                    'http' => array(
                        'header'  => "Content-type: application/x-www-form-urlencoded\r\n",
                        'method'  => 'POST',
                        'content' => http_build_query($data)
                    )
                );
                $context  = stream_context_create($options);
                $reponse = file_get_contents($url, false, $context);
                if ($reponse === FALSE) { /* Handle error */ }

    }

    // Get tables
    $insertQuey = "SHOW TABLES;";
    $result = mysqli_query($con, $insertQuey) or die("4: Getting data Failed");
    if($result -> num_rows > 0){

        while($row = $result->fetch_assoc()){
            
            if($row["Tables_in_trafficlightresults(two)"] == "information"){
                continue;
            }

            $name = explode("_",$row["Tables_in_trafficlightresults(two)"]);

            // Get the data of the table
            $url = 'http://localhost/sqlconnect/GetData.php';
            $data = array('name' => $row["Tables_in_trafficlightresults(two)"]);

            // use key 'http' even if you send the request to https://...
            $options = array(
                'http' => array(
                    'header'  => "Content-type: application/x-www-form-urlencoded\r\n",
                    'method'  => 'POST',
                    'content' => http_build_query($data)
                )
            );
            $context  = stream_context_create($options);
            $reponse = file_get_contents($url, false, $context);
            if ($reponse === FALSE) { /* Handle error */ }

            $data = explode(" ", $reponse);

            $overallAvg = "";
            $avg0 = "";
            $avg1 = "";
            $flow0 = "";
            $flow1 = "";
            $congestionNorth0 = "";
            $congestionWest0 = "";
            $congestionSouth0 = "";
            $congestionEast0 = "";
            $congestionNorth1 = "";
            $congestionWest1 = "";
            $congestionSouth1 = "";
            $congestionEast1 = "";
            foreach($data as $t){
                $info = explode("_", $t);
                if($info[0] == "Overall#AVG#Waiting#time"){
                    $overallAvg = $info[1];
                }else if($info[0] == "AVG#Waiting#time"){
                    $avg0 = $info[1];
                    $avg1 = $info[2];
                }else if($info[0] == "Flow#rate"){
                    $flow0 = $info[1];
                    $flow1 = $info[2];
                }else if($info[0] == "Congestion#north"){
                    $congestionNorth0 = $info[1];
                    $congestionNorth1 = $info[2];
                }else if($info[0] == "Congestion#west"){
                    $congestionWest0 = $info[1];
                    $congestionWest1 = $info[2];
                }else if($info[0] == "Congestion#south"){
                    $congestionSouth0 = $info[1];
                    $congestionSouth1 = $info[2];
                }else if($info[0] == "Congestion#east"){
                    $congestionEast0 = $info[1];
                    $congestionEast1 = $info[2];
                }
            }


            $carnumber = $name[0];
            $method = $name[1];
            $starting = end($name);
            //echo gettype($name) . " ";
            $insertQuey = "INSERT INTO `information`(`algorithm`, `carNumber`, `startingDirection`, `overall#avg`, `avg0`, `avg1`, `flowRate0`, `flowRate1`, `CongestionNorth0`, `CongestionWest0`, `CongestionSouth0`, `CongestionEast0`, `CongestionNorth1`, `CongestionWest1`, `CongestionSouth1`, `CongestionEast1`  )
                           VALUES ('".$method."','". $carnumber."', '". $starting."','". $overallAvg."','". $avg0."','". $avg1."', '". $flow0."','".$flow1."','". $congestionNorth0."', '". $congestionWest0."','". $congestionSouth0."', '". $congestionEast0."','". $congestionNorth1."', '". $congestionWest1."','". $congestionSouth1."', '". $congestionEast1."');";
            mysqli_query($con, $insertQuey) or die("4: Insert Failed");
        }

    }else{
        echo("0");
    }

    /*
    $table = $_POST["table"];
    $name = $_POST["name"];
    $time = $_POST["waiting_time"];
    $streat = $_POST["streat"];
    $turning = $_POST["turningDirection"];
    
    echo ($name." and ".$time." and ".$streat." and ".$turning);

    // add times to the table
    $insertQuey = "INSERT INTO `$table`(`name`, `waitingTime`, `Direction`, `Streat`) VALUES ('".$name."','". $time."', '". $turning."','". $streat."');";
    mysqli_query($con, $insertQuey) or die("4: Insert Failed");

    echo("0");
    */
?>