

<!--
  _        _   _                          _               _                                                     ___  
 | |      (_) | |                        | |             | |                                                   |__ \ 
 | |       _  | | __   ___    __      __ | |__     __ _  | |_     _   _    ___    _   _     ___    ___    ___     ) |
 | |      | | | |/ /  / _ \   \ \ /\ / / | '_ \   / _` | | __|   | | | |  / _ \  | | | |   / __|  / _ \  / _ \   / / 
 | |____  | | |   <  |  __/    \ V  V /  | | | | | (_| | | |_    | |_| | | (_) | | |_| |   \__ \ |  __/ |  __/  |_|  
 |______| |_| |_|\_\  \___|     \_/\_/   |_| |_|  \__,_|  \__|    \__, |  \___/   \__,_|   |___/  \___|  \___|  (_)  
                                                                   __/ |                                             
                                                                  |___/                                              


Like what you see? I'm available to hire during the upcoming summer.
I'm an A-level computer scientist and aspiring software engineer, with a side passion for buses. My expertise are primarily desktop computer programs codded in C#; although I am also confident with Visual Basic, SQL and XAML. My latest work was developing a C# WPF application to teach & test mathematical transformations for a local secondary school; you can find this here: https://github.com/jfoot/Transformations . 

This web page is my first ever attempt coding JavaScript, HTML and CSS and produced in only 2 days. As such I believe I’m an adaptable learner, eager to gain real world experience programming. If you would like to offer me a job, please email me at jonathanfoot@hotmail.co.uk
-->

<!DOCTYPE html>
<html>
  <head>
  <title>Live Reading Buses</title>
  <link rel="icon" href="https://jfoot.github.io/favicon.jpg">
    <style>
       #map {
        height:  600px;
        width: 100%;
        position:  relative;
       }
       #Controls {
            width: 100%;
            z-index: 100;
            position:  relative;
          }
          #FAQ {
            width: 100%;
            height:  600px;
            position:  relative;
          }
 
	    @font-face {
font-family: "RB";
src: url("Assets/Zileetzmtrmjycykknfoirvvvns.ttf");
}
	    
	    @font-face {
font-family: "RBL";
src: url("Assets/Sephoefjgexsocnfpoleqmrvzqm.ttf");
}
	    
	    h1 {
font-family: 'RB', Arial, sans-serif;
font-weight:normal;
font-style:normal;
}
	    	    	    h2 {
font-family: 'RB', Arial, sans-serif;
font-weight:normal;
font-style:normal;
}
	    
	    	    h3 {
font-family: 'RB', Arial, sans-serif;
font-weight:normal;
font-style:normal;
}
	    
	    	    	    p {
font-family: 'RBL', Arial, sans-serif;
font-weight:normal;
font-style:normal;
}

.redtext {
        color: red;
        font-family: 'RB', Arial, sans-serif;
font-weight:normal;
font-style:normal;
}
	    
    </style>
  </head>
  <body>
  
    <h1>Unofficial Live Reading Buses Map</h1>
    <h2 id="LastUpdate"  class="redtext">Locating buses now, please wait...</h2>
    <h3>Please be patient it can take a while to load, especially if your viewing multiple services at once or on a mobile device.</h3>
    <h3>To begin select bellow the bus service(s) you wish to view. The map updates every 10 seconds, if you change the service(s) you wish to view wait for the next map update to reflect these changes or manually click the Force Refresh Map button.</h3>
      

 
    <div id="Controls">
        <button type="button" onclick=initMap() >Force Refresh Map</button>
        <button type="button" onclick=ToggleOn() >Toggle All Services On</button>
        <button type="button" onclick=ToggleOff() >Toggle All Services Off</button>
        <div id="map"/> 
    </div>
  
<!--
        <h2>FAQ</h2>
        <h3>No buses are being displayed why is that?</h3>
        <p>Sorry about that, come back in a little while and try again. Its most likely due to my proxy server provider being overloaded. If your tech savvy open the developer console and tweet me your error messages. On Chrome this is Shift + Ctrl + I</p>
    
        <h3>Hows this working?</h3>
        <p>Good question, its retrieving a raw JSON file from the same server as the Reading Buses Smart phone app. A JSON file contains structured data on objects, such as longitude, latitude, service number and vehicle ID. I take this raw data and then plot it onto a google map using the Google Maps API and Java Script. This whole process is repeated every 10s. If you wish to be super technical you can view the pages source code, this was actually my first ever web page I made.</p>
    
        <h3>Will this be around forever?</h3>
        <p>Theoretically yes, I codded it without the need for any server code, all processing is done client side. However, Buscms (now known as Passengers) are intending to close the API feed I’m using, once they’ve done so this site will also stop working. I'm currently in contact with them for offical usage, therefore, if you like it let me know and give me a tweet, I may be lucky enough to get given an alternative feed.</p>
    
        <p>Legal Mumbo Jumbo Bellow</p>
        <p>Although the data has been produced and processed from sources believed to be reliable, no warranty, expressed or implied, is made regarding accuracy, adequacy, completeness, legality, reliability or usefulness of any information. This disclaimer applies to both isolated and aggregate uses of the information. The information is provided on an "as is" basis.  This is not an official publication and is in no way endorsed by Reading Buses. All company, product and service names used on this site are for identification purposes only and belong to their respective owners.</p>

-->
	  
    <script>
	var myVar = setInterval(initMap, 10000);	//Used to loop the "initmap" function every 10.5s
        var maps;				//Used to define the Google Map
        var markers = [];			//Used to store the map markers.
        var Buses				//Used to store the JSON bus data
        var Ser = ["1", "2", "2a", "3", "4", "5", "6", "6a", "9", "10", "11", "12", "13", "14", "15", "15a", "16", "17", "18", "19a", "19b", "19c", "21", "22", "23", "24", "25", "26", "27", "28", "29", "33", "33a", "40", "50", "50a", "50b", "51", "52", "53", "53a", "53b", "53x", "60", "60a", "60b", "60c", "60e", "60x", "62", "62a", "63", "500", "GW"]; //Used to store all of the known bus services (services with icons)

	//Used to dynamically create the GUI, to save having to code it manually
        function AddDynamicGUI() {
            for (var i = 0; i < (Ser.length); i++) {
                var img = document.createElement('img');
                img.src = 'Assets/' + Ser[i] + '.png';
                img.title = Ser[i];
                document.getElementById('Controls').appendChild(img);

                var CheckBoxs = document.createElement("input");
                CheckBoxs.type = "checkbox";
                CheckBoxs.id = Ser[i];
                document.getElementById('Controls').appendChild(CheckBoxs);
            }
	    //Also creates a button for unknown services
            var img = document.createElement('img');
            img.src = 'Assets/NoImage.png';
            img.title = 'No Icon';
            document.getElementById('Controls').appendChild(img);

            var CheckBoxs = document.createElement("input");
            CheckBoxs.type = "checkbox";
            CheckBoxs.id = 'NoIcon';
            document.getElementById('Controls').appendChild(CheckBoxs);
       
            h2("FAQ");

       
        }

        function h2(text) {
        var h2 = document.createElement('h2');
        h2.appendChild(document.createTextNode(text));
        document.body.appendChild(h2);
}

	//Toggles all of the bus services on
        function ToggleOn() {
          	for (var i = 0; i < (Ser.length); i++) {
                document.getElementById(Ser[i]).checked = true;
            }
            document.getElementById('NoIcon').checked = true;
            initMap();
        }
	//Toggles all of the bus services off
        function ToggleOff() {
            for (var i = 0; i < (Ser.length); i++) {
                document.getElementById(Ser[i]).checked = false;
            }
            document.getElementById('NoIcon').checked = false;
            initMap();
        }

    


	//Inital loading/ creation of the map
        function loadMap() {
            maps = new google.maps.Map(document.getElementById('map'), {
                zoom: 12,
                center: new google.maps.LatLng(51.4547978, -0.9797911)
            });
            //Turns off all labels/ shops (irrelevant details)
		var noPoi = [
                {
                    featureType: "poi",
                    stylers: [
                        { visibility: "off" }
                    ]
                }
            ];
            maps.setOptions({ styles: noPoi });

            //var trafficLayer = new google.maps.TrafficLayer();
            //trafficLayer.setMap(maps);

            AddDynamicGUI();
            initMap();
        }
	
	//The looped function which places the markers onto the Google Map
        function initMap() {
            
		var Httpreq = new XMLHttpRequest(); // a new request
		Httpreq.onreadystatechange = function() {
		    if (this.readyState == 4 && this.status == 200) {
		     
					    //Retrieves the data and turns it into JSON, this is achieved using a proxy to circumvent CORS being blocked.  
	     Buses = JSON.parse(this.responseText);
	     document.getElementById("LastUpdate").innerHTML = 'Last Update: ' + Date();
            
	    //Removes all previous markers
            for (var i = 0; i < markers.length; i++) {
                markers[i].setMap(null);
            }
            markers = [];

  	    //Places all the new markers down - if they are switched on.
            for (var i = 0; i < (Buses.length); i++) {
                if (contains(Buses[i].service) == true) {
                    for (var x = 0; x < (Ser.length); x++) {
                        if ((document.getElementById(Ser[x]).checked && Buses[i].service == Ser[x])) {
                            var infowindow = new google.maps.InfoWindow();
                            var content = '<h3> Service: ' + Buses[i].service + '</h3> <p> Vehicle ID: ' + Buses[i].vehicle + '</p><p> Bearing: ' + Buses[i].bearing + '<p>';
                            var marker = new google.maps.Marker({
                                position: new google.maps.LatLng(Buses[i].latitude, Buses[i].longitude),
                                map: maps,
                                title: Buses[i].service,
                                icon:  ('Assets/' + Buses[i].service + '.png'),
                            });


                            google.maps.event.addListener(marker, 'click', (function (marker, content, infowindow) {
                                return function () {
                                    infowindow.setContent(content);
                                    infowindow.open(map, marker);
                                };
                            })(marker, content, infowindow));
                            markers.push(marker);
                        }

                    }
                } else if (document.getElementById('NoIcon').checked == true) {

                    var infowindow = new google.maps.InfoWindow();
                    var content = '<h3> Service: ' + Buses[i].service + '</h3> <p> Vehicle ID: ' + Buses[i].vehicle + '<p>';
                    var marker = new google.maps.Marker({
                        position: new google.maps.LatLng(Buses[i].latitude, Buses[i].longitude),
                        map: maps,
                        icon: ('Assets/NoImage.png'),
                        title: Buses[i].service,
                    });

                    google.maps.event.addListener(marker, 'click', (function (marker, content, infowindow) {
                        return function () {
                            infowindow.setContent(content);
                            infowindow.open(map, marker);
                        };
                    })(marker, content, infowindow));
                    markers.push(marker);
                }
            }
		    }
		  };
		
		Httpreq.open("GET", 'https://cors-anywhere.herokuapp.com/http://siri.buscms.com/reading/vehicles.json', true);
		Httpreq.send(null);
        }

	//Checks to see if the file/ icon exsits (hence a known service)
	//This will currently output an error to the console everytime false is outtputed- TODO stop this happening.
   
	function contains(obj) {
	    var i = Ser.length;
	    while (i--) {
	       if (Ser[i] === obj) {
		   return true;
	    }
		    }
		    return false;
		}

       
    </script>
    <script async defer
    src="https://maps.googleapis.com/maps/api/js?key=AIzaSyDHM8zxiPWlsmBSTYmyx8nqg2EJjkLPWNM&callback=loadMap">
    </script>
  </body>
</html>
