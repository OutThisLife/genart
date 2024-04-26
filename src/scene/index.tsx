import {
  Environment,
  OrbitControls,
  useAspect,
  useFBO
} from '@react-three/drei'
import {
  Canvas,
  useFrame,
  type RawShaderMaterialProps
} from '@react-three/fiber'
import { Suspense, useMemo, useRef } from 'react'
import * as THREE from 'three'
import fragmentShader from './frag.fs?raw'
import vertexShader from './vert.vs?raw'

function Inner() {
  const scale = useAspect(1, 1)

  const fbo0 = useFBO()
  const fbo1 = useFBO()
  const toggle = useRef(false)

  const args = useMemo(
    () =>
      ({
        uniforms: {
          uResolution: { value: new THREE.Vector4() },
          uTime: { value: 0 },
          uMouse: { value: new THREE.Vector2() },
          uChannel0: { value: new THREE.Texture() }
        },
        fragmentShader,
        vertexShader,
        glslVersion: THREE.GLSL3,
        transparent: true
      }) satisfies RawShaderMaterialProps,
    []
  )

  useFrame(
    ({
      gl,
      camera,
      scene,
      clock,
      pointer,
      size: { width, height },
      viewport: { dpr }
    }) => {
      const readFBO = toggle.current ? fbo1 : fbo0
      const writeFBO = toggle.current ? fbo0 : fbo1

      const w = width * dpr
      const h = height * dpr

      args.uniforms.uTime.value = clock.getElapsedTime()
      args.uniforms.uResolution.value.set(w, h, w / h, dpr)
      args.uniforms.uMouse.value.copy(pointer)
      args.uniforms.uChannel0.value = readFBO.texture

      gl.setRenderTarget(writeFBO)
      gl.render(scene, camera)
      gl.setRenderTarget(null)

      toggle.current = !toggle.current
    }
  )

  return (
    <mesh {...{ scale }}>
      <planeGeometry />
      <rawShaderMaterial {...args} />
    </mesh>
  )
}

export default function Scene() {
  return (
    <Canvas key={Math.random()} dpr={[1, 2]}>
      <color attach="background" args={['#171720']} />
      <fog attach="fog" args={['#171720', 10, 50]} />

      <ambientLight intensity={0.5} />

      <Suspense>
        <Environment preset="city" />
        <Inner />
      </Suspense>

      <OrbitControls />
    </Canvas>
  )
}
