
function OfficeVersion(): String;
var
	found: Boolean;
	ver: String;
	Value: String;

begin
	// Determine Office version
	found := RegQueryStringValue(HKEY_CLASSES_ROOT, 'Excel.Application\CurVer', '', Value);
	// returns: Excel.Application.99, where 99 is the Office internal version
	ver := Copy( Value, 19, 2);
	
	Result := ver;
end;


procedure InstallAddin(AddinFullname: string; BitVersion: Integer);
var
  Count2: Integer;
  
  version: String;
  Value: String;
  optionskey: String;
  ready : Boolean;
  found : Boolean;
  AddinShortname: String;
  ext: String;
  ok: Boolean;
	pf: String;
	x: String;
	ver: String;
	
begin
  Log('-- Install addin: ' + AddinFullname);
  Log('-- BitVersion: ' + IntToStr(BitVersion));

  AddinShortname := ExtractFileName( AddinFullname );
  ext := ExtractFileExt( AddinFullname );

  ver := OfficeVersion();
  version := ver + '.0';
  Log('Office version: ' + version );
  
  if (CompareStr( version, '14.0') < 0) and (BitVersion = 64) then
	begin
		Log('    64-bit not supported before Office 2010');
		exit;
	end;

  if (CompareStr( version, '12.0') < 0) and (CompareText( ext, '.xlam' ) = 0) then
	begin
		Log('    .xlam only for Office 2007 and above');
		exit;
	end;

  if IsWin64 then
  begin
	Log('** 64 bit platform, check install path of Office');
	
	pf := ExpandConstant('{pf64}');
	x:= pf + '\Microsoft Office\Office' + ver + '\EXCEL.EXE';
	Log('    Check install path: ' + x );
	// C:\Program Files\Microsoft Office\Office14
	if ( FileExists(x) ) then
	begin
		// OK
		Log( '    64bit Office version found. ' );
		if BitVersion = 32 then 
			begin
				Log('    32-bit addin not for 64-bit Office. Exitting...');
				exit;
			end
		
	end
	else
	begin
		Log( '    32bit Office version found. ' );
		if BitVersion = 64 then 
			begin
				Log('    64-bit addin not for 32bit Office. Exitting...');
				exit;
			end
		
	end
	
  end
  else
	if BitVersion = 64 then
	begin
		Log('Cannot install 64 addin om 32 bit platform');
		exit
	end;
  
	
  optionskey := 'Software\Microsoft\Office\' + version + '\Excel\Options';
  if RegQueryStringValue(HKEY_CURRENT_USER, optionskey, 'Pos', Value) then
  begin
	  
	Log('   Office version: [' + version  + '] Component: ' + AddinShortname + ' Ext: ' + ext);
 	
	// We have options
	Count2 := 0;
	ready := False;
	// MsgBox('Versie: ' + version, mbInformation, MB_OK);
	repeat
	  if Count2 = 0 then
		found := RegQueryStringValue(HKEY_CURRENT_USER, optionskey, 'OPEN', Value)
	  else
		found := RegQueryStringValue(HKEY_CURRENT_USER, optionskey, 'OPEN' + IntToStr(Count2), Value);

	  
	  if found then
	  begin
		
		// Is this OPEN key same addin as AddinFullname??
		if Pos( AddinShortName, Value ) > 0 then
		begin
		  Log( '   Replace existing...');
		  Log( '   Slot: OPEN' + IntToStr(Count2) );
		  if Count2 = 0 then
			ok := RegWriteStringValue(HKEY_CURRENT_USER, optionskey, 'OPEN', Chr(34) + AddinFullname + Chr(34))
		  else
			ok := RegWriteStringValue(HKEY_CURRENT_USER, optionskey, 'OPEN' + IntToStr(Count2), Chr(34) + AddinFullname + Chr(34));
		  
		  if ok then
			Log('>> OK')
		  else
			Log('>> NOT OK');
			
		  ready := True;
		end;
		Count2 := Count2 + 1;
	  end
	  else
	  begin
		ready := True;
		Log( '   Create new slot...');
		Log( '   Slot: OPEN' + IntToStr(Count2) );
		if Count2 = 0 then
			ok := RegWriteStringValue(HKEY_CURRENT_USER, optionskey, 'OPEN', Chr(34) + AddinFullname + Chr(34))
		  else
			ok := RegWriteStringValue(HKEY_CURRENT_USER, optionskey, 'OPEN' + IntToStr(Count2), Chr(34) + AddinFullname + Chr(34));

		if ok then
			Log('>> OK')
		  else
			Log('>> NOT OK');
			
		Log('   Addin-Manager');
		ok := RegWriteStringValue(HKEY_CURRENT_USER, 'Software\Microsoft\Office\' + version + '\Excel\Add-in Manager', AddinFullname, '');
		if ok then
			Log('>> OK')
		  else
			Log('>> NOT OK');

	  end;
	  
	until ready = True;
  end

      
  
  
end;

procedure UninstallAddin(AddinShortname: string);
var
  versions: TArrayOfString;
  Names: TArrayOfString;
  Count1: Integer;
  Count2: Integer;
  I: Integer;

  version: String;
  Value: String;
  optionskey: String;
  applkey: String;
  ready : Boolean;
  found : Boolean;

begin
  Log( '-- Uninstall addin ' + AddinShortname );
  if RegGetSubkeyNames(HKEY_CURRENT_USER, 'Software\Microsoft\Office', versions) then
  begin


    for Count1 := 0 to GetArrayLength(versions)-1 do
    begin
      version := versions[Count1];
	  
	  
      optionskey := 'Software\Microsoft\Office\' + version + '\Excel\Options';
      if RegQueryStringValue(HKEY_CURRENT_USER, optionskey, 'Pos', Value) then
      begin
        // We have options
        Count2 := 0;
        ready := False;
		//MsgBox('sn: ' + AddinShortName, mbInformation, MB_OK);
		
		if RegGetValueNames(HKEY_CURRENT_USER, optionskey, Names) then
			for I := 0 to GetArrayLength(Names)-1 do
			begin
				if Pos( 'OPEN', Names[I] ) = 1 then
				begin
					found := RegQueryStringValue(HKEY_CURRENT_USER, optionskey, Names[I], Value)
					if Pos( AddinShortName, Value ) > 0 then
					begin
					    Log( '   Remove entry ' + Names[I] );
						RegDeleteValue(HKEY_CURRENT_USER, optionskey, Names[I])
					end;
				end;
			end;
		

		applkey := 'Software\Microsoft\Office\' + version + '\Excel\Add-in Manager';
		// MsgBox(applkey, mbInformation, MB_OK );

        if RegGetValueNames(HKEY_CURRENT_USER, applkey, Names) then
	    // MsgBox('BlaBla:'+IntToStr(GetArrayLength(Names)), mbInformation, MB_OK );
		    for I := 0 to GetArrayLength(Names)-1 do
		    begin
		      //MsgBox(AddinShortName + ' : ' + Names[I], mbInformation, MB_OK );
				if Pos( AddinShortName, Names[I] ) > 0 then
				begin 	
	                //MsgBox( 'Verwijderen : ' + Names[I], mbInformation, MB_OK );
	                Log( '   Remove from Add-in manager.' );
	                RegDeleteValue(HKEY_CURRENT_USER, applkey, Names[I] );
	            end;
		    end;

      end;


    end;     // end of loop versions

  end
  else
  begin
    // add any code to handle failure here
  end;
end;

procedure AddTrustedLocation(Location: string; Description: string);
var
  versions: TArrayOfString;
  Count: Integer;
  Count1: Integer;
  version: String;
  optionskey: String;
  Value: string;
  DWValue: Cardinal;
  found : Boolean;
  ready : Boolean;
  ext: String;
  
begin

  if RegGetSubkeyNames(HKEY_CURRENT_USER, 'Software\Microsoft\Office', versions) then
  begin

    for Count1 := 0 to GetArrayLength(versions)-1 do
    begin
      version := versions[Count1];
	  
      optionskey := 'Software\Microsoft\Office\' + version + '\Excel\Security\Trusted Locations';
      if RegQueryDWordValue(HKEY_CURRENT_USER, 'Software\Microsoft\Office\' + version + '\Excel\Options', 'FirstRun', DWValue) then
      begin
	    Log('Install trusted location: ' + Location + ' Version: ' + version); 
	    if (CompareStr( version, '12.0') >= 0) then
	    begin
		  
		  Count := -1;
		  ready := False;
		  found := False;

		  repeat
			Count := Count + 1;
			found := RegQueryStringValue(HKEY_CURRENT_USER, optionskey + '\Location' + IntToStr(Count), 'Path', Value);
			if found then
			begin
				Log('Found: ' + Location + ' Key: ' + IntToStr(Count));
				if (CompareStr(Value , Location) = 0 ) then
					ready := True
			end
			else
			begin
				Log('Not found: ' + Location + ' Key: ' + IntToStr(Count));
				ready := True
			end
		  until ready;
		  
		  ext := optionskey + '\Location' + IntToStr(Count);
		  if not found then
		  begin
			Log('Install trusted location: ' + Location + ' Key: ' + ext);
			RegWriteDWordValue(HKEY_CURRENT_USER, ext, 'AllowSubfolders', 1);
			RegWriteStringValue(HKEY_CURRENT_USER, ext, 'Date', GetDateTimeString('dd/mm/yyyy hh:nn', '-', ':'));
			RegWriteStringValue(HKEY_CURRENT_USER, ext, 'Description', Description);
			RegWriteStringValue(HKEY_CURRENT_USER, ext, 'Path', Location);
		  end;
	    end;
	  end;
	  
	end;
  end;
end;

procedure EnableAccessVBOM();
var
	ver: String;
	version: String;
	
begin
	// Determine Office version
	ver := OfficeVersion();
	version := ver + '.0';
  
	if (CompareStr( version, '12.0') >= 0) then
		if RegKeyExists(HKEY_CURRENT_USER, 'Software\Microsoft\Office\' + version) then
			RegWriteDWordValue(HKEY_CURRENT_USER, 'Software\Microsoft\Office\' + version + '\Excel\Security', 'AccessVBOM', 1);
		
end;