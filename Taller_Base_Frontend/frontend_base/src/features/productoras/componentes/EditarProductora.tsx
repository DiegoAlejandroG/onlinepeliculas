import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router";
import type { SubmitHandler } from "react-hook-form";
import type ProductoraCreacion from "../modelos/ProductoraCreacion.model";
import type Productora from "../modelos/Productora.model";
import FormularioProductora from "./FormularioProductora";
import Cargando from "../../../componentes/Cargando";
import MostrarErrores from "../../../componentes/MostrarErrores";
import clienteAPI from "../../../api/clienteAxios";
import axios from "axios";
import { extraerErrores } from "../../../utilidades/extraerErrores";
import convertirProductoraAFormData from "../utilidades/convertirProductoraAFormData";

export default function EditarProductora() {
    const { id } = useParams();
    const [modelo, setModelo] = useState<ProductoraCreacion | undefined>(undefined);
    const [errores, setErrores] = useState<string[]>([]);
    const [cargando, setCargando] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        let activo = true;
        setModelo(undefined);
        setErrores([]);
        setCargando(true);

        if (!id) {
            setErrores(['No se encontró el identificador de la productora.']);
            setCargando(false);
            return () => {
                activo = false;
            };
        }

        clienteAPI.get<Productora>(`/productoras/${id}`)
            .then(respuesta => {
                if (!activo) {
                    return;
                }

                setModelo({
                    nombre: respuesta.data.nombre,
                    fechaFundacion: respuesta.data.fechaFundacion,
                    sitioWeb: respuesta.data.sitioWeb,
                    logo: respuesta.data.logo
                });
            })
            .catch(error => {
                if (activo) {
                    setErrores(obtenerErrores(error, 'cargar'));
                }
            })
            .finally(() => {
                if (activo) {
                    setCargando(false);
                }
            });

        return () => {
            activo = false;
        };
    }, [id]);

    const onSubmit: SubmitHandler<ProductoraCreacion> = async (valores) => {
        if (!id) {
            setErrores(['No se encontró el identificador de la productora.']);
            return;
        }

        setErrores([]);

        try {
            const formData = convertirProductoraAFormData(valores);
            await clienteAPI.put(`/productoras/${id}`, formData, {
                headers: { 'Content-Type': 'multipart/form-data' }
            });
            navigate('/productoras');
        } catch (error) {
            setErrores(obtenerErrores(error, 'guardar'));
        }
    }

    return (
        <>
            <h3>Editar Productora</h3>
            {cargando ? <Cargando /> : modelo
                ? <FormularioProductora errores={errores} modelo={modelo} onSubmit={onSubmit} />
                : <MostrarErrores errores={errores} />}
        </>
    )
}

function obtenerErrores(error: unknown, accion: string): string[] {
    if (axios.isAxiosError(error)) {
        const data: unknown = error.response?.data;
        if (typeof data === 'object' && data !== null && 'errors' in data) {
            return extraerErrores(error);
        }

        if (error.response) {
            return [`No se pudo ${accion} la productora (${error.response.status}): ${error.message}`];
        }

        return [`No se pudo conectar con el API para ${accion} la productora: ${error.message}`];
    }

    return [error instanceof Error ? error.message : `Ocurrió un error inesperado al ${accion} la productora.`];
}
