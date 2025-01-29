using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class DominantFrequencyCounter : MonoBehaviour
{
    //Datos reales
    public List<float> data;
    [SerializeField] string fileName;


    public float[] inputData; // Datos de la posición en x respecto al tiempo
    float sampleRate; // Frecuencia de muestreo en segundos
    int maxFFTIndex; //El indice en la FFT donde está el pico (la frecuencia)

    float frequency;
    float totalCadence;
/*
    void Start()
    {
        ReadCSVFile(fileName);

        //Calculo del coste
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();

        //Eliminar hasta el dato 300
        data = data.GetRange(300, data.Count - 300);

        //----------------POR TRAMOS---------------------

        string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + fileName + "_FFT.csv";
        Debug.Log($"Ruta: {filePath}");
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Nombre;Intervalo 128;FFT;Cadencia");

            // Procesar los datos en intervalos de 128
            for (int i = 0; i < data.Count; i += 128)
            {
                // Determinar el tamaño del bloque 
                int blockSize = Mathf.Min(128, data.Count - i);

                // Obtener el bloque actual de datos
                List<float> block = data.GetRange(i, blockSize);

                // Calcular los índices del intervalo
                int startInterval = 300 + i;
                int endInterval = startInterval + blockSize;

                // Imprimir el intervalo
                Debug.Log($"Intervalo: {startInterval}-{endInterval}");

                if (startInterval == 1580)
                {
                    Debug.Log("FIN");

                    //------------------ENTERO----------------------

                    AdjustListLengthToPowerOfTwo(data);
                    inputData = data.ToArray();
                    DoFFT(inputData);
                    totalCadence = frequency * 60f;
                    // Calcula la frecuencia de prueba
                    Debug.Log("----FRECUENCIA TOTAL-----");
                    Debug.Log("[FFT] Frecuencia dominante: " + frequency);
                    Debug.Log("[FFT] Cadencia: " + totalCadence);
                    writer.WriteLine($"Cadencia total;1024(300-1324);{frequency};{totalCadence}");
                    break;
                }
                else
                {
                    inputData = block.ToArray();
                    DoFFT(inputData);
                    // Calcula la frecuencia de prueba
                    Debug.Log("----FRECUENCIA MEDIA DATOS PRUEBA: METODO TRANSFORMADA RÁPIDA DE FOURIER-----");
                    Debug.Log("[FFT] Frecuencia dominante: " + frequency);
                    Debug.Log("[FFT] Cadencia: " + frequency * 60f);
                    writer.WriteLine($"{fileName};{startInterval} - {endInterval};{frequency};{frequency * 60f}");
                }
                
            }

        }

        //stopwatch.Stop();


        // Debug.Log("Coste del algoritmo: " + stopwatch.ElapsedMilliseconds + " ms");
    }
*/
    List<List<float>> SplitList(List<float> list, int chunkSize)
    {
        List<List<float>> chunks = new List<List<float>>();
        int count = list.Count;

        for (int i = 0; i < count; i += chunkSize)
        {
            List<float> chunk = list.GetRange(i, Mathf.Min(chunkSize, count - i));
            chunks.Add(chunk);
        }

        return chunks;
    }

    bool IsPowerOfTwo(int n)
    {
        return (n > 0) && (n & (n - 1)) == 0;
    }

    void AdjustListLengthToPowerOfTwo(List<float> list)
    {
        int originalLength = list.Count;

        if (IsPowerOfTwo(originalLength))
        {
            // La longitud ya es una potencia de 2
            Debug.Log("La longitud de la lista ya es una potencia de 2.");
            return;
        }

        // Encuentra la mayor potencia de 2 menor o igual a la longitud actual
        int newLength = 1;
        while (newLength * 2 <= originalLength)
        {
            newLength *= 2;
        }

        // Recortar la lista
        list.RemoveRange(newLength, list.Count - newLength);

        Debug.Log("Lista recortada a la longitud más cercana que es potencia de 2: " + newLength);
    }

    void ReadCSVFile(string fileName)
    {
        data = new List<float>();
        TextAsset csvData = Resources.Load<TextAsset>(fileName);

        if (csvData == null)
        {
            Debug.LogError("Archivo CSV no encontrado.");
            return;
        }

        using (StringReader reader = new StringReader(csvData.text))
        {
            bool isFirstLine = true;
            int yColumnIndex = -1;

            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                if (isFirstLine)
                {
                    // Procesar la primera línea para encontrar el índice de la columna "Posicion Y"
                    string[] headers = line.Split(';');
                    for (int i = 0; i < headers.Length; i++)
                    {
                        if (headers[i].Trim() == "Posicion Y")
                        {
                            yColumnIndex = i;
                            break;
                        }
                    }
                    isFirstLine = false;

                    if (yColumnIndex == -1)
                    {
                        Debug.LogError("No se encontró la columna 'Posicion Y' en el archivo CSV.");
                        return;
                    }
                }
                else
                {
                    // Leer los datos de las siguientes líneas
                    string[] fields = line.Split(';');
                    if (fields.Length > yColumnIndex)
                    {
                        if (float.TryParse(fields[yColumnIndex], out float yPos))
                        {
                            data.Add(yPos);
                        }
                        else
                        {
                            Debug.LogWarning("No se pudo convertir el valor de 'Posicion Y' a float en la línea: " + line);
                        }
                    }
                }
            }
        }

        // Ahora `yPositions` contiene todas las posiciones Y del archivo CSV
        Debug.Log("Se han cargado " + data.Count + " posiciones Y.");
    }

    public float DoFFT(float[] FFTdata)
    {
        // Realiza la Transformada Rápida de Fourier
        Complex[] complexData = new Complex[FFTdata.Length];
        for (int i = 0; i < FFTdata.Length; i++)
        {
            complexData[i] = new Complex(FFTdata[i], 0); // Los datos deben ser en forma de números complejos
        }
        FFT(complexData);

        //Determinar segundos de muestra. La camara es 30fps
        sampleRate = FFTdata.Length / 30;

        // Encuentra la frecuencia dominante
        float maxMagnitude = 0;
        for (int i = 5; i < complexData.Length / 2; i++) //Se empieza en 5 porque la transformada tiene un pico al principio muy alto
        {
            float magnitude = complexData[i].Magnitude;
            if (magnitude > maxMagnitude)
            {
                maxMagnitude = magnitude;
                maxFFTIndex = i;
            }
        }

        frequency = maxFFTIndex / sampleRate;
       // Debug.Log("----FRECUENCIA MEDIA DATOS: METODO TRANSFORMADA RÁPIDA DE FOURIER-----");
       // Debug.Log("[FFT] Frecuencia dominante: " + frequency);

        return frequency;
    }

    // Implementación de la Transformada Rápida de Fourier (FFT)
    public void FFT(Complex[] data)
    {
        int n = data.Length;
        int m = (int)Mathf.Log(n, 2);

        for (int i = 0; i < n; i++)
        {
            int j = ReverseBits(i, m);
            if (j > i)
            {
                Complex temp = data[i];
                data[i] = data[j];
                data[j] = temp;
            }
        }

        for (int i = 1; i <= m; i++)
        {
            int size = 1 << i;
            int halfSize = size / 2;
            Complex twiddleFactorStep = new Complex(Mathf.Cos(Mathf.PI / halfSize), Mathf.Sin(Mathf.PI / halfSize));

            Complex twiddleFactor = new Complex(1, 0);
            for (int j = 0; j < halfSize; j++)
            {
                for (int k = j; k < n; k += size)
                {
                    int rightIndex = k + halfSize;
                    Complex temp = data[rightIndex] * twiddleFactor;
                    data[rightIndex] = data[k] - temp;
                    data[k] += temp;
                }
                twiddleFactor *= twiddleFactorStep;
            }
        }
    }

    // Función para invertir los bits de un número para FFT
    int ReverseBits(int val, int width)
    {
        int result = 0;
        for (int i = 0; i < width; i++)
        {
            result <<= 1;
            result |= val & 1;
            val >>= 1;
        }
        return result;
    }

    // Clase para representar números complejos
    public class Complex
    {
        public float Real { get; set; }
        public float Imaginary { get; set; }

        public Complex(float real, float imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public float Magnitude
        {
            get { return Mathf.Sqrt(Real * Real + Imaginary * Imaginary); }
        }


        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }

        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Real - b.Real, a.Imaginary - b.Imaginary);
        }

        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.Real * b.Real - a.Imaginary * b.Imaginary,
                               a.Real * b.Imaginary + a.Imaginary * b.Real);
        }
    }
}
