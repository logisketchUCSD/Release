Sketch Recognition Tools 
(Plain text README)


Christine Alvarado
Aaron Wolin, Jason Fennell, Devin Smith, Max Pflueger

Harvey Mudd College
sketchers@cs.hmc.edu

***********************

Required software


To use these tools you must have the following installed:
	- Jounrnal Note Reader Component
	- Tablet PC SDK

Both are available on the Mircrosoft website.

***********************

JntToXml


JntToXml is a console application, run in the following manner:

Usage: JntToXml.exe (-c | -d directory | -r) (-f)
Usage: JntToXml.exe input1.jnt [input2.jnt ...]

-c: convert all files in current directory
-d directory: convert all files in the specified directory
-r: recursively convert files from the current directory
-f: fragment the strokes


After completing the conversion process on a file, JntToXml places the resulting .xml files into a \convertedJnt directory located in the directory that JntToXml was called from.


***********************

Domain Files


Domain files are text (.txt) files of the format:

<name of research group>
<name of domain>
<label> <priority number> <color>
<label> <priority number> <color> ...


Example:

HMC Research 07
Test Wire/Gate Logic Diagram
Wire 0 Blue
Gate 1 Red


***********************

MIT XML Format


Sketches are stored hierarchically:
   * Points are the smallest unit
   * Substrokes are Shapes that are groups of points. They are primitive lines and arcs.
   * Strokes are Shapes that are groups of substrokes.
   * Other Shapes are labels that groups of substroke together with a type (e.g. "And", "Or")


Point
   * Attributes:
      * x: (double) x coordinate
      * y: (double) y coordinate
      * pressure: (int) Pressure for the point (0-255 on Tablet PC)
      * time: (unsigned long) Time point was created in milliseconds since 1/1/1970 UTC
      * id: (GUID) Identifying Guid for the point
      * name: (string) Name of the point

   * Example:
      <point x="6150" y="2374" pressure="127" time="1157483794277" name="point" id="1faa972c-78cd-4221-9609-e7cb784c4f50" />   


Shape
   * Sub-Elements:
      * arg: The components of the shape (e.g., Points that are in the Stroke)
      * alias: The aliases of the shape (names for the components that are used in the language)
   
   * Attributes:
      * id: (GUID) Shape ID
      * name: (string) Name of the shape
      * time: (unsigned long) Time shape was created in milliseconds since 1/1/1970 UTC. Creation time is recorded as the time when the user finally lifts the stylus.
      * type: (string) The type of the shape (e.g., "Stroke")
      * author: (GUID) Id of the author of the stroke
      * color: (RGBA integer) RGBA representation of the color
      * height: (int) Used for the height of a shape
      * width: (int) Used for the width of a shape
      * laysInk: (boolean) If true render the shape, if false, don't.
      * penTip: "Rectangle" or "Ball". The pen tip type.
      * raster: "MaskPen" or "CopyPen". How to render the ink.
      * x: (int) Top left origin, x
      * y: (int) Top left origin, y
      * leftx: (int) Top left origin, x. This is redundant.
      * topy: (int) Top left origin, y. This is redundant.
      * start: (GUID) Starting argument (point, substroke, etc.) based on time
      * end: (GUID) Ending argument based on time
      * source: (string) Where the shape was created

   * Examples:
   <shape type="substroke" name="substroke" id="a6ed61dc-09fc-4370-910b-ff59c5a94d80" time="1157483750968" x="2032" y="1318" color="-16777216" height="651" width="402" penTip="Ball" raster="CopyPen" leftx="2032" topy="1318" start="020113db-580f-488c-94b1-507b35cd0273" end="98900040-af33-4349-b0ac-7423bfa43231" source="Converter">
      <arg type="point">020113db-580f-488c-94b1-507b35cd0273</arg> 
      <arg type="point">3c91af04-edb3-4905-80a0-e5443fce18ef</arg> 
      ...

   <shape type="stroke" name="stroke" id="b96f38c3-c34e-4642-941a-a5b17ef89071" time="1157483794843" source="Converter">
      <arg type="substroke">bef43cdb-a10b-49e0-b42c-104fa143021d</arg>
      ...


***********************

Labeler


Open Sketch:
   * Go to either File->Open Sketch or click on the 'Open Sketch' button on the main window toolbar
   * Navigate to and select the appropriate XML file to load a sketch from    * To load a sketch from a Windows Journal file (.jnt), select the 'Files of type' in the Open Sketch dialog, and select 'Microsoft Windows Journal Files' filter


Save Sketch:
   * Go to either File->Save Sketch or click on the 'Save Sketch' button on the main window toolbar
   * Enter the file name to save the sketch to in the 'File Name' field
   * You can only save a sketch as an XML file, not as a JNT


Load Domain:
   * Go to either File->Load Domain or click on the 'Load Domain' button on the main window toolbar
   * Navigate to and select the appropriate domain file in .txt format


Selecting Substrokes:
   * The labeler supports both a control-click scheme and a substroke lassoing ability for substroke selection
   * Ctrl-click scheme: Click on an unselected substroke to select it. Click on a selected substroke to unselect it.
   * Lasso: Circle the group of substrokes you want to select
   * To clear a selection, click on any whitespace in the main panel


Labeling Substrokes:
   * After selecting a group of substrokes to label, click on the beige button that appears at the bottom right corner of the dashed selection box
   * If a valid domain is loaded, a list of possible labels will appear
   * Scroll down this list box until you see the appropriate label
      * If a check box next to a label is unchecked, then the substroke does not contain this label. Clicking this label will apply the label to the substroke, select the check box, and clear the current selection.
      * If a check box next to a label is checked, then the substroke contains this label. Clicking this label will remove the label from the substroke, unselect the check box, and clear the current selection.
   * To stop labeling a substroke, click on any whitespace in the main panel
   * The topmost box in the label menu contains the previous label applied. Clicking on this label will have the same behavior as any other label.


Fragmenting Strokes:
   * Fragmenting a stroke breaks the stroke up into primitive lines and arcs, called substrokes
   * Auto-fragmentation 
      * If a sketch is loaded, click on the 'Auto Fragment' button on the main toolbar
      * The sketch will be fragmented automatically, where red points indicate fragmentations
      * NOTE: this will clear any previous hand fragmentations you have committed
   * Hand fragmentation
      * Select a substroke that is part of the stroke you want to fragment. If a stroke has not been fragmented, then the stroke = the substroke. The behavior to indicate what stroke a substroke is part of has not been implemented yet.
      * Click on the 'Frag. Stroke' button on the main toolbar
      * A dialog box pops up, displaying the full stroke to fragment and any current fragmentation points.
      * Clear the fragmentation points if you want to delete all fragment points
      * Indicate new fragment points by crossing drawing over the point where you want the stroke to be fragmented
         * A red line indicates where you are drawing
         * After you lift up on the stylus (release the left mouse button) new fragmentation points will appear where your drawing motion intersected the stroke
      * To commit these changes, click 'Done' on the Fragment Stroke dialog box toolbar. If you want to cancel the chances, click the 'Cancel' button
   * NOTE: Any fragmentations made on a stroke whose substrokes contain different labels will remove the labels from the substrokes


KNOWN BUGS
   * Labeling/unlabeling substrokes with more than one label has buggy behavior and can cause the application to crash
   * Substroke selection sometimes requires more clicks than should be necessary to select a substroke or clear a selection
   * Substroke thickening/unthickening does not always clear after the selection has cleared