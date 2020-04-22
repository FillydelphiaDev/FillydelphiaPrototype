# Python 3

import os
import os.path
import subprocess

def convert():
    sfm_folder = input('Enter full path to the SFM folder:\n')
    
    dmx_converter = os.path.join(sfm_folder, 'game', 'bin', 'dmxconvert.exe')
    if not os.path.exists(dmx_converter):
        print('Can\'t find dmxconverter.exe at ' + dmx_converter)
        return
    
    input_folder = input('\nEnter full path to the folder with DMX files:\n')
    if not os.path.exists(input_folder):
        print('Can\'t find such folder!')
        return
    if os.path.isfile(input_folder):
        print('This is not a folder!')
        return
    
    convert_to = input('\nConvert DMX to "binary" or "tex"?\n')
    if convert_to != 'binary' and convert_to != 'tex':
        print('Expected "binary" or "tex"!')
        return
    convert_from = 'tex' if convert_to == 'binary' else 'binary' 
    
    output_folder = os.path.join(input_folder, convert_to)
    if not os.path.exists(output_folder):
        os.mkdir(output_folder)
    
    for f in os.listdir(input_folder):
        input_file = os.path.join(input_folder, f)
        if not os.path.isfile(input_file) or not f.endswith('.dmx'):
            continue
        output_file = os.path.join(output_folder, f)
        
        cmd = '"' + dmx_converter + '" -i "' + input_file + '" -ie ' + convert_from + ' -o "' + output_file + '" -of ' + convert_to
        print(cmd)
        subprocess.call(cmd)

if __name__ == '__main__':
    convert()