<?php
  $client_id = "YOUR_CLIENT_ID";
  $client_secret = "YOUR_CLIENT_SECRET";
  $encText = $_POST["text"];

	function GetMysqlField($table, $where_field, $where_data, $field)
	{
		global $mysqlconn;
		$value = null;
		if ($where_data == "") return "---";
		$stmt = $mysqlconn->prepare("SELECT $field FROM `$table` WHERE `$where_field` = ?");
		$stmt->bind_param("s", $where_data);
		$stmt->execute();
		$result = $stmt->get_result();
		if ($row = $result->fetch_assoc()) 
		{
			$value = $row[$field];
		}
		$stmt->close();
		if ($value == null) return null;
		//return "[캐시]".$value;
		return $value;
	}

	function GetMysqlList($table)
	{
		global $mysqlconn;
		$values = array();
		$stmt = $mysqlconn->prepare("SELECT * FROM `$table`");
		$stmt->execute();
		$result = $stmt->get_result();
		while ($row = $result->fetch_assoc()) 
		{
			array_push($values, $row);
		}
		$stmt->close();
		//return "[캐시]".$value;
		return $values;
	}

	function SetMysql($key, $data)
	{
		global $mysqlconn;
			$stmt = $mysqlconn->prepare("INSERT INTO `cache`(`key`,`data`) VALUES (?, ?)");
			$stmt->bind_param("ss", $key, $data);
			$stmt->execute();

			if ($stmt->errno != 0)
			{echo $stmt->error;
			}
	}
	function t_end()
	{
		global $mysqlconn;
		global $result;

		$mysqlconn->close();
		for($i = 0; $i < count($result);$i++)
		{
			if ($i > 0) echo "\n";
			echo $result[$i];
		}
		exit();
	}
// 문장 분석
	$mysql_setting = new StdClass();
	$mysql_setting->servername = "localhost";
	$mysql_setting->username = "id";
	$mysql_setting->password = "password";
	$mysql_setting->database = "naver_papago";
	$mysqlconn = new mysqli($mysql_setting->servername, $mysql_setting->username, $mysql_setting->password,$mysql_setting->database);

	$keywords = GetMysqlList("keyword");
	foreach($keywords as $key)
	{
		$encText = str_ireplace($key['key'], $key['data'], $encText);
	}

	$lines = explode( "\n", $encText );
	$result = array();

	$no_cache_data = "";
	$no_cache_line = array();
	foreach($lines as $text)
	{
		$data = GetMysqlField("cache", "key", $text, "data");
		array_push($result,$data);
		if ($data == NULL)
		{
			$no_cache_data .= $text;
			$no_cache_data .= "\n";
			array_push($no_cache_line,$text);
		}
	}
	if ($no_cache_data == "")
	{
		t_end();

	}


  $postvars = "source=en&target=ko&text=".urlencode($no_cache_data);
  $url = "https://openapi.naver.com/v1/papago/n2mt";
  $is_post = true;
  $ch = curl_init();
  curl_setopt($ch, CURLOPT_URL, $url);
  curl_setopt($ch, CURLOPT_POST, $is_post);
  curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
  curl_setopt($ch,CURLOPT_POSTFIELDS, $postvars);


  if (!function_exists('getallheaders')) 
{ 
    function getallheaders() 
    { 
       $headers = array (); 
       foreach ($_SERVER as $name => $value) 
       { 
           if (substr($name, 0, 5) == 'HTTP_') 
           { 
               $headers[str_replace(' ', '-', ucwords(strtolower(str_replace('_', ' ', substr($name, 5)))))] = $value; 
           } 
       }
       return $headers; 
    } 
} 


  $nh = getallheaders();
  $headers = array();


//$nh['X-Naver-Client-Id'] = "EQdDwq0ggbqt18XPPSIH";
//$nh['X-Naver-Client-Secret'] = "1aSHg0Eb4r";

  $headers[] = "X-Naver-Client-Id: ".$nh['X-Naver-Client-Id'];
  $headers[] = "X-Naver-Client-Secret: ".$nh['X-Naver-Client-Secret'];
  curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
  $response = curl_exec ($ch);
  $status_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
   // echo "status_code:".$status_code."<br>";
  curl_close ($ch);
  if($status_code == 200) {

	  $object = json_decode($response);


	  $trans_data = explode("\n", $object->message->result->translatedText );
	// 정리 
		$trans_data_i = 0;
		for($i = 0; $i < count($result);$i++)
		{
			if ($result[$i] == null)
			{
				//$result[$i] = "[번역]".$trans_data[$trans_data_i];
				$result[$i] = $trans_data[$trans_data_i];
				SetMysql($no_cache_line[$trans_data_i], $trans_data[$trans_data_i]);
				$trans_data_i++;
			}
		}
		
		t_end();



  } else {
    echo $response;
  }

  
	$mysqlconn->close();
?>