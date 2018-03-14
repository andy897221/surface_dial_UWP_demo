# surface_dial_UWP_demo

a UWP program to demostrate how to program the surface dial interaction on your UWP app, this UWP app is meant for controlling the tplink light bulb using the surface dial, most of the code is about sending GET requests to the server hosting the script on event changes of the surface dial, the server side prgoram is here for your reference: https://github.com/andy897221/tplink-LB120-control-client-server-demo 

the main program logic code is resided in ```Dial-tplink/MainPage.xaml.cs```

Details:

```
private CurrentTool thisTool;
private RadialController radialController;
private RadialControllerConfiguration radialControllerConfig;
private RadialControllerMenuItem ToggleMenuItem;
private RadialControllerMenuItem BrightnessMenuItem;
private RadialControllerMenuItem ColorMenuItem;
```

and

```
InitializeDial()
```

the above code intialize the surface dial menu item, and map the variable to a Enum, such that the state of the surface dial (selected item of the surface dial menu) can be observed via Enum and the required logic can be coded, further comments are written in the code

```
InitializeDial()
```

also mapped functions on event of the surface dial (rotation, click)

the rest of the code are self explantory after understanding ```InitializeDial()```
