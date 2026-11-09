import { useEntidades } from "../../../hooks/useEntidades";
import IndiceEntidades from "../../../componentes/IndiceEntidades";
import type Productora from "../modelos/Productora.model";

export default function IndiceProductoras() {
    const entidadesHook = useEntidades<Productora>('/productoras');

    return (
        <IndiceEntidades<Productora>
            titulo="Productoras"
            nombreEntidad="Productora"
            url="/productoras"
            urlCrear="/productoras/crear"
            {...entidadesHook}
        >
            {(productoras, botones) => <>
                <thead>
                    <tr>
                        <th scope="col">Logo</th>
                        <th scope="col">Nombre</th>
                        <th scope="col">Fecha de fundación</th>
                        <th scope="col">Sitio web</th>
                        <th scope="col" className="text-end">Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    {productoras.map(productora => <tr key={productora.id}>
                        <td>
                            {productora.logo
                                ? <img
                                    src={productora.logo}
                                    alt={`Logo de ${productora.nombre}`}
                                    style={{ maxWidth: 80, maxHeight: 60 }}
                                />
                                : <span>Sin logo</span>}
                        </td>
                        <td>{productora.nombre}</td>
                        <td>{productora.fechaFundacion}</td>
                        <td>
                            <a href={productora.sitioWeb} target="_blank" rel="noopener noreferrer">
                                {productora.sitioWeb}
                            </a>
                        </td>
                        <td className="text-end">
                            {botones(`/productoras/editar/${productora.id}`, productora.id)}
                        </td>
                    </tr>)}
                </tbody>
            </>}
        </IndiceEntidades>
    )
}
